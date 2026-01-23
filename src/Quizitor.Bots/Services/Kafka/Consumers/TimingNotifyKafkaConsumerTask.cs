using System.Text.Json;
using Confluent.Kafka;
using LPlus;
using Microsoft.Extensions.Options;
using Quizitor.Bots.Configuration;
using Quizitor.Bots.Services.TelegramBot.ClientFactory;
using Quizitor.Bots.UI.GameServer;
using Quizitor.Data;
using Quizitor.Data.Extensions;
using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Services.Kafka.Consumers;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class TimingNotifyKafkaConsumerTask(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<KafkaOptions> options,
    ILogger<TimingNotifyKafkaConsumerTask> logger)
    : KafkaConsumerTask(options,
        logger)
{
    private readonly IOptions<KafkaOptions> _options = options;

    protected override Task<KafkaConsumerRunnerDelegate[]> GetConsumerRunners(CancellationToken stoppingToken)
    {
        return Task.FromResult<KafkaConsumerRunnerDelegate[]>([
            CreateConsumerRunner<int, string>(
                KafkaTopics.QuestionTimingNotifyTopicName,
                $"{KafkaTopics.QuestionTimingNotifyTopicName}.{_options.Value.ConsumerGroupId}",
                ProcessAsync)
        ]);
    }

    private Task ProcessAsync(
        ConsumeResult<int, string> result,
        CancellationToken cancellationToken)
    {
        var timingNotifyDto = JsonSerializer.Deserialize<QuestionTimingNotifyDto>(result.Message.Value);
        if (timingNotifyDto is null) return Task.CompletedTask;

        return serviceScopeFactory.ExecuteUnitOfWorkWithRetryAsync(async services =>
            {
                var dbContextProvider = services.GetRequiredService<IDbContextProvider>();
                if (await dbContextProvider
                        .QuestionTimings
                        .GetByIdOrDefaultAsync(
                            timingNotifyDto.TimingId,
                            cancellationToken) is not { } timing) return;

                var question = await dbContextProvider
                    .Questions
                    .GetByIdAsync(
                        timing.QuestionId,
                        cancellationToken);

                var serverTime = await dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);
                if (timing.StopTime != null) return;
                if (timing.StartTime >= serverTime) return;
                var endTime = timing.StartTime.AddSeconds(question.Time);
                var delta = endTime - serverTime;
                if (delta < TimeSpan.Zero) return;

                var telegramBotClientFactory = services.GetRequiredService<ITelegramBotClientFactory>();
                var updateContext = new UpdateContext(null, new Update(), timingNotifyDto.InitiatedAt, false);

                var participants = await dbContextProvider
                    .Users
                    .GetBySessionIdAsync(
                        timing.SessionId,
                        cancellationToken);

                var text = string.Format(
                    TR.L + "_GAME_SERVER_QUESTION_NOTIFICATION_TXT",
                    question.NotificationTime);

                var options = await dbContextProvider
                    .Questions
                    .GetOptionsByQuestionIdAsync(
                        question.Id,
                        cancellationToken);

                var keyboard = Keyboards.Options(options);

                foreach (var participant in participants)
                {
                    if (!participant.GameServerId.HasValue ||
                        await dbContextProvider
                            .Bots
                            .GetByIdOrDefaultAsync(
                                participant.GameServerId.Value,
                                cancellationToken) is not { } bot) continue;

                    await telegramBotClientFactory
                        .CreateForBot(bot)
                        .SendMessage(
                            updateContext,
                            participant.Id,
                            text,
                            ParseMode.Html,
                            replyMarkup: keyboard,
                            cancellationToken: cancellationToken);
                }

                var round = await dbContextProvider
                    .Rounds
                    .GetByIdAsync(
                        question.RoundId,
                        cancellationToken);

                var adminText = string.Format(
                    TR.L + "_GAME_ADMIN_QUESTION_NOTIFICATION_TXT",
                    round.Title,
                    question.Title,
                    question.NotificationTime);

                var adminBots = await dbContextProvider
                    .Bots
                    .GetActiveGameAdminsAsync(cancellationToken);

                var adminUsers = await dbContextProvider
                    .Users
                    .GetGameAdminsBySessionIdAsync(
                        timing.SessionId,
                        TelegramBotConfiguration.AuthorizedUserIds,
                        cancellationToken);

                var adminBotClients = adminBots
                    .ToDictionary(
                        x => x.Id,
                        telegramBotClientFactory.CreateForBot);

                foreach (var adminUser in adminUsers)
                {
                    if (!adminUser.GameAdminId.HasValue ||
                        !adminBotClients.TryGetValue(adminUser.GameAdminId.Value, out var client)) continue;

                    await client
                        .SendMessage(
                            updateContext,
                            adminUser.Id,
                            adminText,
                            ParseMode.Html,
                            cancellationToken: cancellationToken);
                }
            },
            cancellationToken);
    }
}