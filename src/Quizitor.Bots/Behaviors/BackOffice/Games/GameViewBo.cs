using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Games.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Games;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IGameViewBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<IGameViewBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class GameViewBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<IGameViewBackOfficeContext>,
    Behavior
{
    /// <summary>
    ///     <b>game</b>.{gameId}.{gamePageNumber}
    /// </summary>
    public const string Button = "game";

    public override string[] Permissions => [UserPermission.BackOfficeGameView];

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
                    TR.L + "_BACKOFFICE_GAME_VIEW_TXT",
                    context.Base.Game.Title.EscapeHtml()),
                ParseMode.Html,
                replyMarkup: Keyboards.GameView(
                    context.Base.Game.Id,
                    context.Base.GamePageNumber), cancellationToken: cancellationToken);
    }

    protected override async Task<IGameViewBackOfficeContext?> PrepareContextAsync(
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
            return IGameViewBackOfficeContext.Create(
                game,
                gamePageNumber,
                backOfficeContext);
        }

        return null;
    }
}