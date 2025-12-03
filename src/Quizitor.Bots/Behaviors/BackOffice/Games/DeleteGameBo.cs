using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Games.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Games;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IDeleteGameBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<IDeleteGameBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class DeleteGameBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<IDeleteGameBackOfficeContext>,
    Behavior
{
    /// <summary>
    ///     <b>deletegame</b>.{gameId}.{gamePageNumber}
    /// </summary>
    public const string Button = "deletegame";

    public override string[] Permissions => [UserPermission.BackOfficeGameDelete];

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
                    TR.L + "_BACKOFFICE_DELETE_GAME_CONFIRMATION_TXT",
                    context.Base.Game.Title.Html,
                    context.Base.RoundsCount,
                    context.Base.QuestionsCount,
                    context.Base.OptionsCount,
                    context.Base.RulesCount,
                    context.Base.SessionsCount,
                    context.Base.SubmissionsCount,
                    context.Base.QuestionTimingsCount,
                    context.Base.TimingNotifyEventsCount,
                    context.Base.TimingStopEventsCount,
                    context.Base.ConnectedUsersCount),
                ParseMode.Html,
                replyMarkup: Keyboards.DeleteGame(
                    context.Base.Game.Id,
                    context.Base.GamePageNumber),
                cancellationToken: cancellationToken);
    }

    protected override async Task<IDeleteGameBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } gameIdString,
                {
                } gamePageNumberString
            ] &&
            int.TryParse(gameIdString, out var gameId) &&
            int.TryParse(gamePageNumberString, out var gamePageNumber) &&
            await dbContextProvider.Games.GetByIdOrDefaultAsync(gameId, cancellationToken) is { } game)
        {
            var roundsCount = await dbContextProvider
                .Rounds
                .CountByGameIdAsync(
                    game.Id,
                    cancellationToken);

            var questionsCount = await dbContextProvider
                .Questions
                .CountByGameIdAsync(
                    game.Id,
                    cancellationToken);

            var optionsCount = await dbContextProvider
                .Questions
                .CountOptionsByGameIdAsync(
                    game.Id,
                    cancellationToken);

            var rulesCount = await dbContextProvider
                .Questions
                .CountRulesByGameIdAsync(
                    game.Id,
                    cancellationToken);

            var sessionsCount = await dbContextProvider
                .Sessions
                .CountByGameIdAsync(
                    game.Id,
                    cancellationToken);

            var submissionsCount = await dbContextProvider
                .Submissions
                .CountByGameIdAsync(
                    game.Id,
                    cancellationToken);

            var questionTimingsCount = await dbContextProvider
                .QuestionTimings
                .CountByGameIdAsync(
                    game.Id,
                    cancellationToken);

            var timingNotifyEventsCount = await dbContextProvider
                .QuestionNotifyTimings
                .CountByGameIdAsync(
                    game.Id,
                    cancellationToken);

            var timingStopEventsCount = await dbContextProvider
                .QuestionStopTimings
                .CountByGameIdAsync(
                    game.Id,
                    cancellationToken);

            var connectedUsersCount = await dbContextProvider
                .Users
                .CountByGameIdAsync(
                    game.Id,
                    cancellationToken);

            return IDeleteGameBackOfficeContext.Create(
                game,
                gamePageNumber,
                roundsCount,
                questionsCount,
                optionsCount,
                rulesCount,
                sessionsCount,
                submissionsCount,
                questionTimingsCount,
                timingNotifyEventsCount,
                timingStopEventsCount,
                connectedUsersCount,
                backOfficeContext);
        }

        return null;
    }
}