using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Games.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Behaviors.Infrastructure.MessageText;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Games;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IGameListBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<IGameListBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class GameListBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<IGameListBackOfficeContext>,
    Behavior
{
    public const int PageSize = 10;

    /// <summary>
    ///     <b>games</b>.{gamePageNumber}
    /// </summary>
    public const string Button = "games";

    public override string[] Permissions => [UserPermission.BackOfficeGameList];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context, cancellationToken);
    }

    protected override async Task<IGameListBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } gamePageNumberString
            ] &&
            int.TryParse(gamePageNumberString, out var gamePageNumber))
        {
            var games = await dbContextProvider
                .Games
                .GetPaginatedAsync(
                    gamePageNumber,
                    PageSize,
                    cancellationToken);

            var gamesCount = await dbContextProvider
                .Games
                .CountAsync(cancellationToken);

            var gamePageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(gamesCount) / PageSize));

            return IGameListBackOfficeContext.Create(
                games,
                gamesCount,
                gamePageNumber,
                gamePageCount,
                backOfficeContext);
        }

        return null;
    }

    public static Task ResponseAsync<TContext>(
        TContext? context,
        CancellationToken cancellationToken)
        where TContext : IBehaviorTraitContext<IGameListBackOfficeContext>
    {
        if (context is null) return Task.CompletedTask;

        var text = string.Format(
            TR.L + "_BACKOFFICE_GAMES_TXT",
            context.Base.GamesCount);
        var keyboard = Keyboards.GameList(
            context.Base.Games,
            context.Base.GamePageNumber,
            context.Base.GamePageCount);

        return context switch
        {
            ICallbackQueryDataPrefixContext callbackQueryDataPrefixContext =>
                context
                    .Base
                    .Client
                    .EditMessageText(
                        context.Base.UpdateContext,
                        context.Base.TelegramUser.Id,
                        callbackQueryDataPrefixContext.MessageId,
                        text,
                        ParseMode.Html,
                        replyMarkup: keyboard,
                        cancellationToken: cancellationToken),
            IMessageTextContext =>
                context
                    .Base
                    .Client
                    .SendMessage(
                        context.Base.UpdateContext,
                        context.Base.TelegramUser.Id,
                        text,
                        ParseMode.Html,
                        replyMarkup: keyboard,
                        cancellationToken: cancellationToken),
            _ => Task.CompletedTask
        };
    }
}