using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Prometheus;
using Quizitor.Bots.Services.TelegramBot.ClientFactory;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Bots.Services.Updates;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Extensions;
using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Services.Kafka.Consumers;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class UpdatesInboundKafkaConsumerTask(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<KafkaOptions> options,
    ILogger<UpdatesInboundKafkaConsumerTask> logger)
    : KafkaConsumerTask(options,
        logger)
{
    private static readonly Histogram BotsUpdateBackOfficeHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}bots_update_backoffice",
        "Histogram of backoffice update handling.",
        new HistogramConfiguration
        {
            Buckets =
            [
                .05, .1, .2, .3, .4, .5, .6, .7, .8, .9, 1, 2, 5, 10, 30, 60
            ],
            LabelNames =
            [
                "is_test"
            ]
        });

    private static readonly Histogram BotsUpdateBotHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}bots_update_bot",
        "Histogram of bot's update handling.",
        new HistogramConfiguration
        {
            Buckets =
            [
                .05, .1, .2, .3, .4, .5, .6, .7, .8, .9, 1, 2, 5, 10, 30, 60
            ],
            LabelNames =
            [
                "bot_id",
                "is_test"
            ]
        });

    private readonly IOptions<KafkaOptions> _options = options;

    protected override async Task<KafkaConsumerRunnerDelegate[]> GetConsumerRunners(CancellationToken stoppingToken)
    {
        var tasks = new List<KafkaConsumerRunnerDelegate>();
        using var serviceScope = serviceScopeFactory.CreateScope();
        var dbContextProvider = serviceScope.ServiceProvider.GetRequiredService<IDbContextProvider>();
        var bots = await dbContextProvider
            .Bots
            .GetAllAsync(stoppingToken);

        foreach (var bot in bots)
        {
            var topic = string.Format(
                KafkaTopics.UpdateBotTopicName,
                bot.Id);
            var groupId = $"{topic}.{_options.Value.ConsumerGroupId}";
            tasks.Add(
                CreateConsumerRunner<long, string>(
                    topic,
                    groupId,
                    (result, token) =>
                        ProcessAsync(bot.Id, result, token)));
        }

        tasks.Add(CreateConsumerRunner<long, string>(
            KafkaTopics.UpdateTopicName,
            $"{KafkaTopics.UpdateTopicName}.{_options.Value.ConsumerGroupId}",
            (result, token) =>
                ProcessAsync(null, result, token)));

        return [.. tasks];
    }

    private async Task ProcessAsync(
        int? botId,
        ConsumeResult<long, string> result,
        CancellationToken cancellationToken)
    {
        if (result.Message?.Value is null ||
            JsonSerializerHelper.TryDeserialize<UpdateContext>(result.Message.Value, JsonBotAPI.Options) is not { } updateContext)
            return;

        using (botId.HasValue
                   ? BotsUpdateBotHistogram.WithLabels(
                   [
                       botId.Value.ToString(),
                       updateContext.IsTest.ToString()
                   ]).NewTimer()
                   : BotsUpdateBackOfficeHistogram.WithLabels([updateContext.IsTest.ToString()]).NewTimer())
        {
            await serviceScopeFactory.ExecuteUnitOfWorkWithRetryAsync(async asyncScope =>
                {
                    var dbContextProvider = asyncScope.ServiceProvider.GetRequiredService<IDbContextProvider>();
                    var updateService = asyncScope.ServiceProvider.GetRequiredService<IUpdateService>();

                    if (botId.HasValue)
                    {
                        if (await dbContextProvider.Bots.GetByIdOrDefaultAsync(botId, cancellationToken) is not { } currentBot)
                        {
                            throw new InvalidOperationException("Bot does not exist.");
                        }

                        if (!currentBot.IsActive) throw new RetryLaterExtension(10000, "Bot is not active.");

                        await updateService.HandleAsync(
                            updateContext,
                            currentBot,
                            cancellationToken);
                    }
                    else
                    {
                        await updateService.HandleAsync(
                            updateContext,
                            null,
                            cancellationToken);
                    }
                },
                cancellationToken);

            await using var asyncScope = serviceScopeFactory.CreateAsyncScope();
            var dbContextProvider = asyncScope.ServiceProvider.GetRequiredService<IDbContextProvider>();
            var telegramBotClientFactory = asyncScope.ServiceProvider.GetRequiredService<ITelegramBotClientFactory>();
            var telegramBotClient = botId.HasValue
                ? await dbContextProvider.Bots.GetByIdOrDefaultAsync(botId.Value, cancellationToken) is { } bot
                    ? telegramBotClientFactory.CreateForBot(bot)
                    : throw new InvalidOperationException($"Unable to get bot {botId}")
                : telegramBotClientFactory.CreateDefault();
            await HandlePostActionAsync(updateContext, telegramBotClient, cancellationToken);
        }
    }

    private static async Task HandlePostActionAsync(
        UpdateContext updateContext,
        ITelegramBotClientWrapper client,
        CancellationToken cancellationToken)
    {
        await (updateContext.Update.Type switch
        {
            UpdateType.CallbackQuery when updateContext.Update.CallbackQuery is { } callbackQuery =>
                client
                    .AnswerCallbackQuery(
                        updateContext,
                        callbackQuery.Id,
                        cancellationToken: cancellationToken),
            _ => Task.CompletedTask
        });
    }
}