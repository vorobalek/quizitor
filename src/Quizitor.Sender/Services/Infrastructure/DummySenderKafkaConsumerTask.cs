using System.Net;
using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Prometheus;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Extensions;
using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;
using Quizitor.Sender.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace Quizitor.Sender.Services.Infrastructure;

internal abstract partial class DummySenderKafkaConsumerTask<TKey>(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<KafkaOptions> options,
    ILogger logger) :
    KafkaConsumerTask(
        options,
        logger)
{
    private readonly ILogger _logger = logger;
    private readonly IOptions<KafkaOptions> _options = options;

    protected abstract string Method { get; }
    protected abstract string TopicMain { get; }
    protected abstract string TopicBot { get; }

    protected abstract Histogram BackOfficeHistogram { get; }
    protected abstract Histogram BotHistogram { get; }

    protected override async Task<KafkaConsumerRunnerDelegate[]> GetConsumerRunners(CancellationToken stoppingToken)
    {
        var processes = new List<KafkaConsumerRunnerDelegate>();
        using var serviceScope = serviceScopeFactory.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<IDbContextProvider>();
        var bots = await dbContext
            .Bots
            .GetAllAsync(stoppingToken);

        foreach (var bot in bots)
        {
            var topic = string.Format(
                TopicBot,
                bot.Id);
            var groupId = $"{topic}.{_options.Value.ConsumerGroupId}";
            processes.Add(CreateConsumerRunner<TKey, string>(
                topic,
                groupId,
                (result, token) =>
                    ProcessAsync(bot.Id, result, token)));
        }

        processes.Add(CreateConsumerRunner<TKey, string>(
            TopicMain,
            $"{TopicMain}.{_options.Value.ConsumerGroupId}",
            (result, token) =>
                ProcessAsync(null, result, token)));

        return [.. processes];
    }

    private async Task ProcessAsync(
        int? botId,
        ConsumeResult<TKey, string> result,
        CancellationToken cancellationToken)
    {
        if (result.Message?.Value is null ||
            JsonSerializerHelper.TryDeserialize<SenderContext>(result.Message.Value, JsonBotAPI.Options) is not { } senderContext) return;

        using (botId.HasValue
                   ? BotHistogram.WithLabels(
                   [
                       botId.Value.ToString(),
                       senderContext.UpdateContext.IsTest.ToString()
                   ]).NewTimer()
                   : BackOfficeHistogram.WithLabels([senderContext.UpdateContext.IsTest.ToString()]).NewTimer())
        {
            await serviceScopeFactory.ExecuteUnitOfWorkWithRetryAsync(async asyncScope =>
                {
                    if (!senderContext.UpdateContext.IsTest)
                    {
                        var dbContextProvider = asyncScope.ServiceProvider.GetRequiredService<IDbContextProvider>();
                        var httpClientFactory = asyncScope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

                        if (botId.HasValue)
                        {
                            if (await dbContextProvider.Bots.GetByIdOrDefaultAsync(botId, cancellationToken) is not { } currentBot)
                            {
                                throw new InvalidOperationException("Bot does not exist.");
                            }

                            using var httpClient = httpClientFactory.CreateClient(currentBot.Id.ToString());
                            await SendRequestAsync(
                                httpClient,
                                currentBot.Token,
                                senderContext.Content,
                                cancellationToken);
                        }
                        else
                        {
                            using var httpClient = httpClientFactory.CreateClient("BackOffice");
                            await SendRequestAsync(
                                httpClient,
                                TelegramBotConfiguration.BotToken,
                                senderContext.Content,
                                cancellationToken);
                        }
                    }

                    await PostProcessAsync(
                        asyncScope,
                        senderContext,
                        cancellationToken);
                },
                cancellationToken);
        }
    }

    protected virtual async Task SendRequestAsync(
        HttpClient httpClient,
        string botToken,
        string content,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync(
            $"https://api.telegram.org/bot{botToken}/{Method}",
            new StringContent(content, Encoding.UTF8, "application/json"),
            cancellationToken);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var responseContent = (await response
                .Content
                .ReadFromJsonAsync<ApiResponse>(
                    JsonBotAPI.Options,
                    cancellationToken))!;
            var exception = new ApiRequestException(
                responseContent.Description!,
                responseContent.ErrorCode,
                responseContent.Parameters);
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            LogTelegramApiRequestFailed(exception);
            if (responseContent.ErrorCode == 429)
                throw new RetryLaterExtension(1000, "429 status code has been found");
        }
    }

    protected virtual Task PostProcessAsync(
        AsyncServiceScope asyncScope,
        SenderContext senderContext,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Error, "Telegram API request failed")]
    partial void LogTelegramApiRequestFailed(Exception exception);
}