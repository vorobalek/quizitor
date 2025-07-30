using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Games.Rounds.Questions.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Rounds.Questions;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IQuestionBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<IQuestionBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal class QuestionViewBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<IQuestionBackOfficeContext>,
    Behavior
{
    /// <summary>
    ///     <b>question</b>.{questionId}.{gamePageNumber}.{roundPageNumber}.{questionPageNumber}
    /// </summary>
    public const string Button = "question";

    public override string[] Permissions => [UserPermission.BackOfficeQuestionView];

    protected virtual string ButtonInternal => Button;

    public string CallbackQueryDataPrefixValue => $"{ButtonInternal}.";

    public virtual Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context, cancellationToken);
    }

    protected override async Task<IQuestionBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } questionIdString,
                {
                } gamePageNumberString,
                {
                } roundPageNumberString,
                {
                } questionPageNumberString
            ] &&
            int.TryParse(questionIdString, out var questionId) &&
            int.TryParse(gamePageNumberString, out var gamePageNumber) &&
            int.TryParse(roundPageNumberString, out var roundPageNumber) &&
            int.TryParse(questionPageNumberString, out var questionPageNumber) &&
            await dbContextProvider.Questions.GetByIdOrDefaultAsync(questionId, cancellationToken) is { } question)
        {
            var round = await dbContextProvider
                .Rounds
                .GetByIdAsync(
                    question.RoundId,
                    cancellationToken);

            var game = await dbContextProvider
                .Games
                .GetByIdAsync(
                    round.GameId,
                    cancellationToken);

            var options = await dbContextProvider
                .Questions
                .GetOptionsByQuestionIdAsync(
                    question.Id,
                    cancellationToken);

            var rules = await dbContextProvider
                .Questions
                .GetRulesByQuestionIdAsync(
                    question.Id,
                    cancellationToken);

            return IQuestionBackOfficeContext.Create(
                question,
                round,
                game,
                options,
                rules,
                gamePageNumber,
                roundPageNumber,
                questionPageNumber,
                backOfficeContext);
        }

        return null;
    }

    protected static Task ResponseAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return context
            .Base
            .Client
            .EditMessageText(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                context.MessageId,
                string.Format(
                    TR.L + "_BACKOFFICE_QUESTION_VIEW_TXT",
                    context.Base.Game.Title.EscapeHtml(),
                    context.Base.Round.Title.EscapeHtml(),
                    context.Base.Question.Title.EscapeHtml(),
                    context.Base.Question.Text,
                    context.Base.Question.Attempts,
                    context.Base.Question.Time,
                    context.Base.Question.NotificationTime.HasValue
                        ? string.Format(
                            TR.L + "_BACKOFFICE_QUESTION_NOTIFICATION_TIME_TXT",
                            context.Base.Question.NotificationTime.Value)
                        : TR.L + "_SHARED_NO_TXT",
                    context.Base.Question.AutoClose
                        ? TR.L + "_SHARED_YES_TXT"
                        : TR.L + "_SHARED_NO_TXT",
                    TR.L + $"_BACKOFFICE_QUESTION_SUBMISSION_NOTIFICATION_TYPE_{context.Base.Question.SubmissionNotificationType}",
                    context.Base.Question.Comment is { } comment
                        ? comment.EscapeHtml()
                        : TR.L + "_SHARED_NO_TXT",
                    context.Base.Options.Length > 0
                        ? string.Join(
                            string.Empty,
                            context.Base.Options
                                .Select(option => string.Format(
                                    TR.L + "_BACKOFFICE_QUESTION_OPTION_LIST_ITEM_TXT",
                                    option.Cost > 0
                                        ? TR.L + "_BACKOFFICE_QUESTION_OPTION_CORRECT_SIGN"
                                        : TR.L + "_BACKOFFICE_QUESTION_OPTION_WRONG_SIGN",
                                    option.Number,
                                    option.Text.EscapeHtml(),
                                    option.Cost)))
                        : TR.L + "_SHARED_NO_TXT",
                    context.Base.Rules.Length > 0
                        ? string.Join(
                            string.Empty,
                            context.Base.Rules
                                .Select(rule => string.Format(
                                    TR.L + "_BACKOFFICE_QUESTION_RULE_LIST_ITEM_TXT",
                                    string.Format(
                                        TR.L + $"_BACKOFFICE_QUESTION_RULE_{rule.GetType().Name}",
                                        rule.GetBackOfficeLocalizationArgs()),
                                    rule.Cost)))
                        : TR.L + "_SHARED_NO_TXT"),
                ParseMode.Html,
                replyMarkup: Keyboards.QuestionView(
                    context.Base.Question,
                    context.Base.GamePageNumber,
                    context.Base.RoundPageNumber,
                    context.Base.QuestionPageNumber),
                cancellationToken: cancellationToken);
    }
}