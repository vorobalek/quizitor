using LPlus;
using Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Configuration;
using Quizitor.Bots.Services.TelegramBot.ClientFactory;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Bots.UI.GameServer;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Data.Entities.Events;
using Quizitor.Data.Enums;
using Quizitor.Kafka.Contracts;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IQuestionStopGameAdminContext>;
using Context = ICallbackQueryDataPrefixContext<IQuestionStopGameAdminContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class QuestionStopGa(
    ITelegramBotClientFactory telegramBotClientFactory,
    IDbContextProvider dbContextProvider) :
    GameAdminBehavior<IQuestionStopGameAdminContext>(
        dbContextProvider),
    Behavior
{
    /// <summary>
    ///     <b>questionstop</b>.{timingId}.{roundPageNumber}.{questionPageNumber}
    /// </summary>
    public const string Button = "questionstop";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string[] GameAdminPermissions =>
    [
        UserPermission.GameAdminQuestionStop,
        UserPermission.GameAdminQuestionView
    ];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public async Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        if (context.Base.CurrentTiming.StopTime is not null)
        {
            await context
                .Base
                .Client
                .AnswerCallbackQuery(
                    context.Base.UpdateContext,
                    context.CallbackQueryId,
                    string.Format(
                        TR.L + "_GAME_ADMIN_QUESTION_NOT_STARTED_TXT",
                        context.Base.Round.Title,
                        context.Base.Question.Title),
                    true,
                    cancellationToken: cancellationToken);
            await context
                .Base
                .Client
                .DeleteMessage(
                    context.Base.UpdateContext,
                    context.Base.TelegramUser.Id,
                    context.MessageId,
                    cancellationToken);
            return;
        }

        await StopAndNotifyAsync(
            _dbContextProvider,
            telegramBotClientFactory,
            context.Base.Client,
            context.Base.UpdateContext,
            context.CallbackQueryId,
            context.Base.CurrentTiming,
            context.Base.TimingNotify,
            context.Base.TimingStop,
            context.Base.GameBots,
            context.Base.GameUsers,
            context.Base.AdminBots,
            context.Base.AdminUsers,
            context.Base.Round,
            context.Base.Question,
            context.Base.NextQuestion,
            context.Base.RoundPageNumber,
            context.Base.QuestionPageNumber,
            context.Base.ServerTime,
            cancellationToken);
        _dbContextProvider
            .AddPostCommitTask(async () =>
                await QuestionViewGa.ResponseAsync(
                    context.Base,
                    context.MessageId,
                    cancellationToken,
                    true));
    }

    protected override async Task<IQuestionStopGameAdminContext?> PrepareGameAdminInternalAsync(
        IGameAdminContext gameAdminContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(gameAdminContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } timingIdString,
                {
                } roundPageNumberString,
                {
                } questionPageNumberString
            ] &&
            int.TryParse(timingIdString, out var timingId) &&
            int.TryParse(roundPageNumberString, out var roundPageNumber) &&
            int.TryParse(questionPageNumberString, out var questionPageNumber) &&
            await _dbContextProvider.QuestionTimings.GetByIdOrDefaultAsync(timingId, cancellationToken) is { } timing)
        {
            var question = await _dbContextProvider
                .Questions
                .GetByIdAsync(
                    timing.QuestionId,
                    cancellationToken);

            var options = await _dbContextProvider
                .Questions
                .GetOptionsByQuestionIdAsync(
                    question.Id,
                    cancellationToken);

            var rules = await _dbContextProvider
                .Questions
                .GetRulesByQuestionIdAsync(
                    question.Id,
                    cancellationToken);

            var round = await _dbContextProvider
                .Rounds
                .GetByIdAsync(
                    question.RoundId,
                    cancellationToken);

            var timingSession = await _dbContextProvider
                .Sessions
                .GetByIdAsync(
                    timing.SessionId,
                    cancellationToken);

            var timingNotify = await _dbContextProvider
                .QuestionNotifyTimings
                .GetByTimingIdOrDefaultAsync(
                    timing.Id,
                    cancellationToken);

            var timingStop = await _dbContextProvider
                .QuestionStopTimings
                .GetByTimingIdOrDefaultAsync(
                    timing.Id,
                    cancellationToken);

            var orderedQuestions = await _dbContextProvider
                .Questions
                .GetOrderedByGameIdAsync(
                    round.GameId,
                    cancellationToken);

            Question? nextQuestion = null;
            for (var i = 0; i < orderedQuestions.Length; i++)
                if (orderedQuestions[i].Id == question.Id && i + 1 < orderedQuestions.Length)
                    nextQuestion = orderedQuestions[i + 1];

            var gameBots = await _dbContextProvider
                .Bots
                .GetActiveGameServersAsync(cancellationToken);

            var gameUsers = await _dbContextProvider
                .Users
                .GetBySessionIdAsync(
                    timingSession.Id,
                    cancellationToken);

            var adminBots = await _dbContextProvider
                .Bots
                .GetActiveGameAdminsAsync(cancellationToken);

            var adminUsers = await _dbContextProvider
                .Users
                .GetGameAdminsBySessionIdAsync(
                    timingSession.Id,
                    TelegramBotConfiguration.AuthorizedUserIds,
                    cancellationToken);

            var serverTime = await _dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);

            return IQuestionStopGameAdminContext.Create(
                timing,
                timingNotify,
                timingStop,
                nextQuestion,
                gameBots,
                gameUsers,
                adminBots,
                adminUsers,
                serverTime,
                IQuestionGameAdminContext.Create(
                    timingSession,
                    round,
                    question,
                    options,
                    rules,
                    timing,
                    roundPageNumber,
                    questionPageNumber,
                    gameAdminContext));
        }

        return null;
    }

    public static async Task StopAndNotifyAsync(
        IDbContextProvider dbContextProvider,
        ITelegramBotClientFactory telegramBotClientFactory,
        ITelegramBotClientWrapper? baseClient,
        UpdateContext updateContext,
        string? callbackQueryId,
        QuestionTiming timing,
        QuestionTimingNotify? timingNotify,
        QuestionTimingStop? timingStop,
        Bot[] gameBots,
        User[] gameUsers,
        Bot[] adminBots,
        User[] adminUsers,
        Round round,
        Question question,
        Question? nextQuestion,
        int roundPageNumber,
        int questionPageNumber,
        DateTimeOffset serverTime,
        CancellationToken cancellationToken)
    {
        if (timing.StopTime is null)
        {
            timing.StopTime = serverTime;
            await dbContextProvider.QuestionTimings.UpdateAsync(timing, cancellationToken);

            if (timingNotify is not null)
                await dbContextProvider
                    .QuestionNotifyTimings
                    .RemoveAsync(
                        timingNotify,
                        cancellationToken);

            if (timingStop is not null)
                await dbContextProvider
                    .QuestionStopTimings
                    .RemoveAsync(
                        timingStop,
                        cancellationToken);

            var text = TR.L + "_GAME_SERVER_QUESTION_STOP_TXT";
            var keyboard = Keyboards.Remove;

            var gameBotClients = gameBots
                .ToDictionary(
                    x => x.Id,
                    telegramBotClientFactory.CreateForBot);
            foreach (var gameUser in gameUsers)
            {
                if (!gameUser.GameServerId.HasValue ||
                    !gameBotClients.TryGetValue(gameUser.GameServerId.Value, out var client)) continue;

                await dbContextProvider
                    .Users
                    .RemovePromptByUserIdBotIdIfExistsAsync(
                        gameUser.Id,
                        gameUser.GameServerId,
                        UserPromptType.GameServerAnswer,
                        cancellationToken);

                dbContextProvider
                    .AddPostCommitTask(async () =>
                        await client
                            .SendMessage(
                                updateContext,
                                gameUser.Id,
                                text,
                                ParseMode.Html,
                                replyMarkup: keyboard,
                                cancellationToken: cancellationToken));
            }

            var adminText = string.Format(
                TR.L + "_GAME_ADMIN_QUESTION_STOPPED_TXT",
                round.Title.EscapeHtml(),
                question.Title.EscapeHtml());

            var nextQuestionText = nextQuestion is not null
                ? string.Format(
                    TR.L + "_GAME_ADMIN_NEXT_QUESTION_TXT",
                    (await dbContextProvider
                        .Rounds
                        .GetByIdAsync(
                            nextQuestion.RoundId,
                            cancellationToken)).Title.EscapeHtml(),
                    nextQuestion.Title.EscapeHtml())
                : TR.L + "_GAME_ADMIN_NO_NEXT_QUESTION_TXT";

            var nextQuestionKeyboard = UI.GameAdmin.Keyboards.QuestionStopNotification(
                nextQuestion?.Id,
                roundPageNumber,
                questionPageNumber);

            var adminBotClients = adminBots
                .ToDictionary(
                    x => x.Id,
                    telegramBotClientFactory.CreateForBot);

            foreach (var adminUser in adminUsers)
            {
                if (!adminUser.GameAdminId.HasValue ||
                    !adminBotClients.TryGetValue(adminUser.GameAdminId.Value, out var client)) continue;

                dbContextProvider
                    .AddPostCommitTask(async () =>
                    {
                        await client
                            .SendMessage(
                                updateContext,
                                adminUser.Id,
                                adminText,
                                ParseMode.Html,
                                cancellationToken: cancellationToken);
                        await client
                            .SendMessage(
                                updateContext,
                                adminUser.Id,
                                nextQuestionText,
                                ParseMode.Html,
                                replyMarkup: nextQuestionKeyboard,
                                cancellationToken: cancellationToken);
                    });
            }

            if (callbackQueryId is not null && baseClient is not null)
                dbContextProvider
                    .AddPostCommitTask(async () =>
                        await baseClient
                            .AnswerCallbackQuery(
                                updateContext,
                                callbackQueryId,
                                string.Format(
                                    TR.L + "_GAME_ADMIN_QUESTION_STOPPED_CLB",
                                    round.Title,
                                    question.Title),
                                true,
                                cancellationToken: cancellationToken));
        }
    }
}