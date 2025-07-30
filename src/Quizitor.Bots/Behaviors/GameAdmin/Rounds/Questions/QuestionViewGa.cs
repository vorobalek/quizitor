using LPlus;
using Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.GameAdmin;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IQuestionGameAdminContext>;
using Context = ICallbackQueryDataPrefixContext<IQuestionGameAdminContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class QuestionViewGa(
    IDbContextProvider dbContextProvider) :
    GameAdminBehavior<IQuestionGameAdminContext>(dbContextProvider),
    Behavior
{
    /// <summary>
    ///     <b>question</b>.{questionId}.{roundPageNumber}.{questionPageNumber}
    /// </summary>
    public const string Button = "question";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string[] GameAdminPermissions => [UserPermission.GameAdminQuestionView];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(
            context.Base,
            context.MessageId,
            cancellationToken);
    }

    protected override async Task<IQuestionGameAdminContext?> PrepareGameAdminInternalAsync(
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

            var timing = await _dbContextProvider
                .QuestionTimings
                .GetActiveBySessionIdOrDefaultAsync(
                    gameAdminContext.Session.Id,
                    cancellationToken);

            var rules = await _dbContextProvider
                .Questions
                .GetRulesByQuestionIdAsync(
                    question.Id,
                    cancellationToken);

            return IQuestionGameAdminContext.Create(
                gameAdminContext.Session,
                round,
                question,
                options,
                rules,
                timing,
                roundPageNumber,
                questionPageNumber,
                gameAdminContext);
        }

        return null;
    }

    public static Task ResponseAsync(
        IQuestionGameAdminContext context,
        int messageId,
        CancellationToken cancellationToken,
        bool withoutKeyboard = false)
    {
        return context
            .Client
            .EditMessageText(
                context.UpdateContext,
                context.TelegramUser.Id,
                messageId,
                string.Format(
                    TR.L + "_GAME_ADMIN_QUESTION_VIEW_TXT",
                    context.Round.Title.EscapeHtml(),
                    context.Question.Title.EscapeHtml(),
                    context.Question.Text,
                    context.Question.Attempts,
                    context.Question.Time,
                    context.Question.NotificationTime.HasValue
                        ? string.Format(
                            TR.L + "_GAME_ADMIN_QUESTION_NOTIFICATION_TIME_TXT",
                            context.Question.NotificationTime.Value)
                        : TR.L + "_SHARED_NO_TXT",
                    context.Question.AutoClose
                        ? TR.L + "_SHARED_YES_TXT"
                        : TR.L + "_SHARED_NO_TXT",
                    TR.L + $"_GAME_ADMIN_QUESTION_SUBMISSION_NOTIFICATION_TYPE_{context.Question.SubmissionNotificationType}",
                    context.Question.Comment is { } comment
                        ? comment.EscapeHtml()
                        : TR.L + "_SHARED_NO_TXT",
                    context.Options.Length > 0
                        ? string.Join(
                            string.Empty,
                            context.Options
                                .Select(option => string.Format(
                                    TR.L + "_GAME_ADMIN_QUESTION_OPTION_LIST_ITEM_TXT",
                                    option.Cost > 0
                                        ? TR.L + "_GAME_ADMIN_QUESTION_OPTION_CORRECT_SIGN"
                                        : TR.L + "_GAME_ADMIN_QUESTION_OPTION_WRONG_SIGN",
                                    option.Number,
                                    option.Text.EscapeHtml(),
                                    option.Cost)))
                        : TR.L + "_SHARED_NO_TXT",
                    context.Rules.Length > 0
                        ? string.Join(
                            string.Empty,
                            context.Rules
                                .Select(rule => string.Format(
                                    TR.L + "_GAME_ADMIN_QUESTION_RULE_LIST_ITEM_TXT",
                                    string.Format(
                                        TR.L + $"_GAME_ADMIN_QUESTION_RULE_{rule.GetType().Name}",
                                        rule.GetGameAdminLocalizationArgs()),
                                    rule.Cost)))
                        : TR.L + "_SHARED_NO_TXT"),
                ParseMode.Html,
                replyMarkup: withoutKeyboard
                    ? null
                    : Keyboards.QuestionView(
                        context.Question.Id,
                        context.Timing?.Id,
                        context.Round.Id,
                        context.RoundPageNumber,
                        context.QuestionPageNumber),
                cancellationToken: cancellationToken);
    }
}