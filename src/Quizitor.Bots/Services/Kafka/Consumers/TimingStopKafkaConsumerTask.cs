using System.Text.Json;
using Confluent.Kafka;
using LPlus;
using Microsoft.Extensions.Options;
using Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions;
using Quizitor.Bots.Configuration;
using Quizitor.Bots.Services.TelegramBot.ClientFactory;
using Quizitor.Bots.UI.GameAdmin;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Data.Extensions;
using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Services.Kafka.Consumers;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class TimingStopKafkaConsumerTask(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<KafkaOptions> options,
    ILogger<TimingStopKafkaConsumerTask> logger)
    : KafkaConsumerTask(options,
        logger)
{
    private readonly IOptions<KafkaOptions> _options = options;

    protected override Task<KafkaConsumerRunnerDelegate[]> GetConsumerRunners(CancellationToken stoppingToken)
    {
        return Task.FromResult<KafkaConsumerRunnerDelegate[]>([
            CreateConsumerRunner<int, string>(
                KafkaTopics.QuestionTimingStopTopicName,
                $"{KafkaTopics.QuestionTimingStopTopicName}.{_options.Value.ConsumerGroupId}",
                ProcessAsync)
        ]);
    }

    private Task ProcessAsync(
        ConsumeResult<int, string> result,
        CancellationToken cancellationToken)
    {
        var timingStopDto = JsonSerializer.Deserialize<QuestionTimingStopDto>(result.Message.Value);
        if (timingStopDto is null) return Task.CompletedTask;

        return serviceScopeFactory.ExecuteUnitOfWorkWithRetryAsync(async asyncScope =>
            {
                var dbContextProvider = asyncScope.ServiceProvider.GetRequiredService<IDbContextProvider>();
                if (await dbContextProvider
                        .QuestionTimings
                        .GetByIdOrDefaultAsync(
                            timingStopDto.TimingId,
                            cancellationToken) is not { } timing) return;

                var serverTime = await dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);
                if (timing.StopTime != null) return;
                if (timing.StartTime >= serverTime) return;
                var telegramBotClientFactory = asyncScope.ServiceProvider.GetRequiredService<ITelegramBotClientFactory>();
                var updateContext = new UpdateContext(null, new Update(), timingStopDto.InitiatedAt, false);

                var question = await dbContextProvider
                    .Questions
                    .GetByIdAsync(
                        timing.QuestionId,
                        cancellationToken);

                if (!question.AutoClose)
                {
                    var round = await dbContextProvider
                        .Rounds
                        .GetByIdAsync(
                            question.RoundId,
                            cancellationToken);

                    var text = string.Format(
                        TR.L + "_GAME_ADMIN_QUESTION_READY_TO_STOP_TXT",
                        round.Title.Html,
                        question.Title.Html);

                    var keyboard = Keyboards.ActiveQuestionCallbacks(
                        round.Title,
                        question.Title,
                        timing.Id,
                        timingStopDto.RoundPageNumber,
                        timingStopDto.QuestionPageNumber);

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
                                text,
                                ParseMode.Html,
                                replyMarkup: keyboard,
                                cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    var round = await dbContextProvider
                        .Rounds
                        .GetByIdAsync(
                            question.RoundId,
                            cancellationToken);

                    var session = await dbContextProvider
                        .Sessions
                        .GetByIdAsync(
                            timing.SessionId,
                            cancellationToken);

                    var timingNotify = await dbContextProvider
                        .QuestionNotifyTimings
                        .GetByTimingIdOrDefaultAsync(
                            timing.Id,
                            cancellationToken);

                    var timingStop = await dbContextProvider
                        .QuestionStopTimings
                        .GetByTimingIdOrDefaultAsync(
                            timing.Id,
                            cancellationToken);

                    var orderedQuestions = await dbContextProvider
                        .Questions
                        .GetOrderedByGameIdAsync(
                            round.GameId,
                            cancellationToken);

                    Question? nextQuestion = null;
                    for (var i = 0; i < orderedQuestions.Length; i++)
                        if (orderedQuestions[i].Id == question.Id && i + 1 < orderedQuestions.Length)
                            nextQuestion = orderedQuestions[i + 1];

                    var gameBots = await dbContextProvider
                        .Bots
                        .GetActiveGameServersAsync(cancellationToken);

                    var gameUsers = await dbContextProvider
                        .Users
                        .GetBySessionIdAsync(
                            session.Id,
                            cancellationToken);

                    var adminBots = await dbContextProvider
                        .Bots
                        .GetActiveGameAdminsAsync(cancellationToken);

                    var adminUsers = await dbContextProvider
                        .Users
                        .GetGameAdminsBySessionIdAsync(
                            session.Id,
                            TelegramBotConfiguration.AuthorizedUserIds,
                            cancellationToken);
                    await QuestionStopGa
                        .StopAndNotifyAsync(
                            dbContextProvider,
                            telegramBotClientFactory,
                            null,
                            updateContext,
                            null,
                            timing,
                            timingNotify,
                            timingStop,
                            gameBots,
                            gameUsers,
                            adminBots,
                            adminUsers,
                            round,
                            question,
                            nextQuestion,
                            timingStopDto.RoundPageNumber,
                            timingStopDto.QuestionPageNumber,
                            serverTime,
                            cancellationToken);
                }
            },
            cancellationToken);
    }
}