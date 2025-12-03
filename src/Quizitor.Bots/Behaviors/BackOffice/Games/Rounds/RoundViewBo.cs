using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Games.Rounds.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Rounds;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IRoundViewBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<IRoundViewBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class RoundViewBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<IRoundViewBackOfficeContext>,
    Behavior
{
    private const int PageSize = 10;

    /// <summary>
    ///     <b>round</b>.{roundId}.{gamePageNumber}.{roundPageNumber}.{questionPageNumber}
    /// </summary>
    public const string Button = "round";

    public override string[] Permissions =>
    [
        UserPermission.BackOfficeRoundView,
        UserPermission.BackOfficeQuestionList
    ];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context, cancellationToken);
    }

    protected override async Task<IRoundViewBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } roundIdString,
                {
                } gamePageNumberString,
                {
                } roundPageNumberString,
                {
                } questionPageNumberString
            ] &&
            int.TryParse(roundIdString, out var roundId) &&
            int.TryParse(gamePageNumberString, out var gamePageNumber) &&
            int.TryParse(roundPageNumberString, out var roundPageNumber) &&
            int.TryParse(questionPageNumberString, out var questionPageNumber) &&
            await dbContextProvider.Rounds.GetByIdOrDefaultAsync(roundId, cancellationToken) is { } round)
        {
            var game = await dbContextProvider
                .Games
                .GetByIdAsync(
                    round.GameId,
                    cancellationToken);

            var questions = await dbContextProvider
                .Questions
                .GetByRoundIdAsync(
                    round.Id,
                    cancellationToken);

            var questionPageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(
                        await dbContextProvider
                            .Questions
                            .CountByRoundIdAsync(
                                round.Id,
                                cancellationToken)) / PageSize));

            return IRoundViewBackOfficeContext.Create(
                round,
                game,
                questions,
                gamePageNumber,
                roundPageNumber,
                questionPageNumber,
                questionPageCount,
                backOfficeContext);
        }

        return null;
    }

    private static Task ResponseAsync(
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
                    TR.L + "_BACKOFFICE_ROUND_VIEW_TXT",
                    context.Base.Game.Title.Html,
                    context.Base.Round.Title.Html,
                    context.Base.Round.Description is { } description
                        ? description.Html
                        : TR.L + "_SHARED_NO_TXT",
                    string.Join(
                        Environment.NewLine,
                        context.Base.Questions
                            .Select(question =>
                                string.Format(
                                    TR.L + "_BACKOFFICE_QUESTION_LIST_ITEM_TXT",
                                    question.Number,
                                    question.Title.Html,
                                    question.Time)))),
                ParseMode.Html,
                replyMarkup: Keyboards.RoundView(
                    context.Base.Game.Id,
                    context.Base.Round.Id,
                    context.Base.Questions,
                    context.Base.GamePageNumber,
                    context.Base.RoundPageNumber,
                    context.Base.QuestionPageNumber,
                    context.Base.QuestionPageCount), cancellationToken: cancellationToken);
    }
}