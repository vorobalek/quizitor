using System.Text.Json;
using LPlus;
using Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Configuration;
using Quizitor.Bots.Services.TelegramBot.ClientFactory;
using Quizitor.Bots.UI.GameServer;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Data.Entities.Events;
using Quizitor.Data.Enums;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IQuestionStartGameAdminContext>;
using Context = ICallbackQueryDataPrefixContext<IQuestionStartGameAdminContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class QuestionStartGa(
    ITelegramBotClientFactory telegramBotClientFactory,
    IDbContextProvider dbContextProvider) :
    GameAdminBehavior<IQuestionStartGameAdminContext>(dbContextProvider),
    Behavior
{
    /// <summary>
    ///     <b>questionstart</b>.{questionId}.{roundPageNumber}.{questionPageNumber}
    /// </summary>
    public const string Button = "questionstart";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string[] GameAdminPermissions =>
    [
        UserPermission.GameAdminQuestionView,
        UserPermission.GameAdminQuestionStart
    ];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public async Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        if (await FallbackIfAnotherQuestionIsActiveAsync(context, cancellationToken)) return;

        var timing = await CreateTimingAndEventsAsync(context, cancellationToken);
        await SetClientPromptTypeAsync(context, timing, cancellationToken);
        await BroadcastQuestionAsync(context, cancellationToken);
        await NotifyGameAdminsAsync(context, timing, cancellationToken);
        await NotifyInitiatorAsync(context, cancellationToken);
        await QuestionViewGa.ResponseAsync(
            context.Base,
            context.MessageId,
            cancellationToken,
            true);
    }

    protected override async Task<IQuestionStartGameAdminContext?> PrepareGameAdminInternalAsync(
        IGameAdminContext gameAdminContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(gameAdminContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } questionIdString,
                {
                } roundPageNumberString,
                {
                } questionPageNumberString
            ] &&
            int.TryParse(questionIdString, out var questionId) &&
            int.TryParse(roundPageNumberString, out var roundPageNumber) &&
            int.TryParse(questionPageNumberString, out var questionPageNumber) &&
            await _dbContextProvider.Questions.GetByIdOrDefaultAsync(questionId, cancellationToken) is { } question)
        {
            var round = await _dbContextProvider
                .Rounds
                .GetByIdAsync(
                    question.RoundId,
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

            var timing = await _dbContextProvider
                .QuestionTimings
                .GetActiveBySessionIdOrDefaultAsync(
                    gameAdminContext.Session.Id,
                    cancellationToken);

            var activeQuestion = await _dbContextProvider
                .Questions
                .GetActiveBySessionIdOrDefaultAsync(
                    gameAdminContext.Session.Id,
                    cancellationToken);

            Round? activeRound = null;
            if (activeQuestion is not null)
                activeRound = await _dbContextProvider
                    .Rounds
                    .GetByIdAsync(
                        activeQuestion.RoundId,
                        cancellationToken);

            var gameBots = await _dbContextProvider
                .Bots
                .GetActiveGameServersAsync(cancellationToken);

            var gameUsers = await _dbContextProvider
                .Users
                .GetBySessionIdAsync(
                    gameAdminContext.Session.Id,
                    cancellationToken);

            var adminBots = await _dbContextProvider
                .Bots
                .GetActiveGameAdminsAsync(cancellationToken);

            var adminUsers = await _dbContextProvider
                .Users
                .GetGameAdminsBySessionIdAsync(
                    gameAdminContext.Session.Id,
                    TelegramBotConfiguration.AuthorizedUserIds,
                    cancellationToken);

            var serverTime = await _dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);

            return IQuestionStartGameAdminContext.Create(
                activeRound,
                activeQuestion,
                gameBots,
                gameUsers,
                adminBots,
                adminUsers,
                serverTime,
                IQuestionGameAdminContext.Create(
                    gameAdminContext.Session,
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

    private static async Task<bool> FallbackIfAnotherQuestionIsActiveAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        if (context.Base.ActiveRound is null || context.Base.ActiveQuestion is null) return false;
        await context
            .Base
            .Client
            .AnswerCallbackQuery(
                context.Base.UpdateContext,
                context.CallbackQueryId,
                string.Format(
                    TR.L + "_GAME_ADMIN_STOP_QUESTION_BEFORE_STARTING_NEW_TXT",
                    context.Base.ActiveRound.Title.EscapeHtml(),
                    context.Base.ActiveQuestion.Title.EscapeHtml()),
                true,
                cancellationToken: cancellationToken);
        return true;
    }

    private async Task<QuestionTiming> CreateTimingAndEventsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        var timing = new QuestionTiming
        {
            Session = context.Base.Session,
            Question = context.Base.Question,
            StartTime = context.Base.ServerTime
        };
        await _dbContextProvider
            .QuestionTimings
            .AddAsync(timing, cancellationToken);

        await _dbContextProvider
            .QuestionStopTimings
            .AddAsync(new QuestionTimingStop
                {
                    Timing = timing,
                    RunAt = context.Base.ServerTime.Add(TimeSpan.FromSeconds(context.Base.Question.Time)),
                    RoundPageNumber = context.Base.RoundPageNumber,
                    QuestionPageNumber = context.Base.QuestionPageNumber
                },
                cancellationToken);

        if (context.Base.Question.NotificationTime.HasValue)
            await _dbContextProvider
                .QuestionNotifyTimings
                .AddAsync(new QuestionTimingNotify
                    {
                        Timing = timing,
                        RunAt = context.Base.ServerTime.Add(
                            TimeSpan.FromSeconds(
                                context.Base.Question.Time
                                - context.Base.Question.NotificationTime.Value))
                    },
                    cancellationToken);

        return timing;
    }

    private async Task SetClientPromptTypeAsync(
        Context context,
        QuestionTiming timing,
        CancellationToken cancellationToken)
    {
        foreach (var gameUser in context.Base.GameUsers)
        {
            await _dbContextProvider
                .Users
                .SetPromptByUserIdBotIdAsync(
                    gameUser.Id,
                    gameUser.GameServerId,
                    UserPromptType.GameServerAnswer,
                    JsonSerializer.Serialize(
                        new GameServerAnswerPromptSubject(timing.Id)),
                    cancellationToken);
        }
    }

    private Task BroadcastQuestionAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        var text = string.Format(
            TR.L + "_GAME_SERVER_QUESTION_START_TXT",
            context.Base.Question.Text,
            context.Base.Question.Time);

        var keyboard = Keyboards.Options(context.Base.Options);

        var gameBotClients = context.Base.GameBots
            .ToDictionary(
                x => x.Id,
                telegramBotClientFactory.CreateForBot);

        foreach (var gameUser in context.Base.GameUsers)
        {
            if (!gameUser.GameServerId.HasValue ||
                !gameBotClients.TryGetValue(gameUser.GameServerId.Value, out var client)) continue;

            _dbContextProvider
                .AddPostCommitTask(async () =>
                    await client
                        .SendMessage(
                            context.Base.UpdateContext,
                            gameUser.Id,
                            text,
                            ParseMode.Html,
                            replyMarkup: keyboard,
                            cancellationToken: cancellationToken));
        }

        return Task.CompletedTask;
    }

    private Task NotifyGameAdminsAsync(
        Context context,
        QuestionTiming timing,
        CancellationToken cancellationToken)
    {
        var text = string.Format(
            TR.L + "_GAME_ADMIN_QUESTION_STARTED_TXT",
            context.Base.Round.Title.EscapeHtml(),
            context.Base.Question.Title.EscapeHtml());

        var actionText = string.Format(
            TR.L + "_GAME_ADMIN_QUESTION_ACTION_TXT",
            context.Base.Round.Title.EscapeHtml(),
            context.Base.Question.Title.EscapeHtml());

        var actionKeyboard = UI.GameAdmin.Keyboards.ActiveQuestionCallbacks(
            context.Base.Round.Title,
            context.Base.Question.Title,
            timing.Id,
            context.Base.RoundPageNumber,
            context.Base.QuestionPageNumber);

        var adminBotClients = context.Base.AdminBots
            .ToDictionary(
                x => x.Id,
                telegramBotClientFactory.CreateForBot);

        foreach (var adminUser in context.Base.AdminUsers)
        {
            if (!adminUser.GameAdminId.HasValue ||
                !adminBotClients.TryGetValue(adminUser.GameAdminId.Value, out var client)) continue;

            _dbContextProvider
                .AddPostCommitTask(async () =>
                    await client
                        .SendMessage(
                            context.Base.UpdateContext,
                            adminUser.Id,
                            text,
                            ParseMode.Html,
                            cancellationToken: cancellationToken));
            _dbContextProvider
                .AddPostCommitTask(async () =>
                    await client
                        .SendMessage(
                            context.Base.UpdateContext,
                            adminUser.Id,
                            actionText,
                            ParseMode.Html,
                            replyMarkup: actionKeyboard,
                            cancellationToken: cancellationToken));
        }

        return Task.CompletedTask;
    }

    private Task NotifyInitiatorAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        _dbContextProvider
            .AddPostCommitTask(async () =>
                await context
                    .Base
                    .Client
                    .AnswerCallbackQuery(
                        context.Base.UpdateContext,
                        context.CallbackQueryId,
                        string.Format(
                            TR.L + "_GAME_ADMIN_QUESTION_STARTED_CLB",
                            context.Base.Round.Title,
                            context.Base.Question.Title),
                        true,
                        cancellationToken: cancellationToken));
        return Task.CompletedTask;
    }
}