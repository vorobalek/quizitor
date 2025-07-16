using LPlus;
using Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Data;
using Quizitor.Data.Entities;

namespace Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IQuestionTimeGameAdminContext>;
using Context = ICallbackQueryDataPrefixContext<IQuestionTimeGameAdminContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class QuestionTimeGa(
    IDbContextProvider dbContextProvider) :
    GameAdminBehavior<IQuestionTimeGameAdminContext>(
        dbContextProvider),
    Behavior
{
    /// <summary>
    ///     <b>time</b>.{timingId}
    /// </summary>
    public const string Button = "time";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string[] GameAdminPermissions => [UserPermission.GameAdminQuestionView];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public async Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        if (context.Base.QuestionTiming.StopTime is not null)
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

        await context
            .Base
            .Client
            .AnswerCallbackQuery(
                context.Base.UpdateContext,
                context.CallbackQueryId,
                string.Format(
                    TR.L + "_GAME_ADMIN_QUESTION_ESTIMATING_TIME_TXT",
                    context.Base.Round.Title,
                    context.Base.Question.Title,
                    Math.Floor((context
                        .Base
                        .QuestionTiming
                        .StartTime
                        .AddSeconds(context.Base.Question.Time) - context.Base.ServerTime).TotalSeconds)
                ),
                true,
                cancellationToken: cancellationToken);
    }

    protected override async Task<IQuestionTimeGameAdminContext?> PrepareGameAdminInternalAsync(
        IGameAdminContext gameAdminContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(gameAdminContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } timingIdString
            ] &&
            int.TryParse(timingIdString, out var timingId) &&
            await _dbContextProvider.QuestionTimings.GetByIdOrDefaultAsync(timingId, cancellationToken) is { } timing)
        {
            var question = await _dbContextProvider
                .Questions
                .GetByIdAsync(
                    timing.QuestionId,
                    cancellationToken);
            var round = await _dbContextProvider
                .Rounds
                .GetByIdAsync(
                    question.RoundId,
                    cancellationToken);

            var serverTime = await _dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);

            return IQuestionTimeGameAdminContext.Create(
                round,
                question,
                timing,
                serverTime,
                gameAdminContext);
        }

        return null;
    }
}