using LPlus;
using Quizitor.Bots.Behaviors.GameAdmin.Rounds.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.GameAdmin;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameAdmin.Rounds;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IRoundGameAdminContext>;
using Context = ICallbackQueryDataPrefixContext<IRoundGameAdminContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class RoundViewGa(IDbContextProvider dbContextProvider) :
    GameAdminBehavior<IRoundGameAdminContext>(dbContextProvider),
    Behavior
{
    private const int PageSize = 10;
    public const string Button = "round";
    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string[] GameAdminPermissions =>
    [
        UserPermission.GameAdminRoundView,
        UserPermission.GameAdminQuestionList
    ];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public Task PerformCallbackQueryDataPrefixAsync(
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
                    TR.L + "_GAME_ADMIN_ROUND_VIEW_TXT",
                    context.Base.Round.Title.Html,
                    context.Base.Round.Description is { } description
                        ? description.Html
                        : TR.L + "_SHARED_NO_TXT",
                    string.Join(
                        Environment.NewLine,
                        context.Base.Questions
                            .Select(question =>
                                string.Format(
                                    TR.L + "_GAME_ADMIN_QUESTION_LIST_ITEM_TXT",
                                    question.Number,
                                    question.Title.Html,
                                    question.Time)))),
                ParseMode.Html,
                replyMarkup: Keyboards.RoundView(
                    context.Base.Round.Id,
                    context.Base.Questions,
                    context.Base.QuestionPageNumber,
                    context.Base.QuestionPageCount,
                    context.Base.RoundPageNumber), cancellationToken: cancellationToken);
    }

    protected override async Task<IRoundGameAdminContext?> PrepareGameAdminInternalAsync(
        IGameAdminContext gameAdminContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(gameAdminContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } roundIdString,
                {
                } roundPageNumberString,
                {
                } questionPageNumberString
            ] &&
            int.TryParse(roundIdString, out var roundId) &&
            int.TryParse(roundPageNumberString, out var roundPageNumber) &&
            int.TryParse(questionPageNumberString, out var questionPageNumber) &&
            await _dbContextProvider.Rounds.GetByIdOrDefaultAsync(roundId, cancellationToken) is { } round)
        {
            var questions = await _dbContextProvider
                .Questions
                .GetByRoundIdAsync(
                    round.Id,
                    cancellationToken);

            var questionPageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(
                        await _dbContextProvider
                            .Questions
                            .CountByRoundIdAsync(
                                round.Id,
                                cancellationToken)) / PageSize));

            return IRoundGameAdminContext.Create(
                round,
                questions,
                roundPageNumber,
                questionPageNumber,
                questionPageCount,
                gameAdminContext);
        }

        return null;
    }
}