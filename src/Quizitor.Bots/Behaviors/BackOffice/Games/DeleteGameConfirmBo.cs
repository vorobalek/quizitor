using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Games.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;

namespace Quizitor.Bots.Behaviors.BackOffice.Games;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IDeleteGameConfirmBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<IDeleteGameConfirmBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class DeleteGameConfirmBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<IDeleteGameConfirmBackOfficeContext>,
    Behavior
{
    /// <summary>
    ///     <b>deletegameconfirm</b>.{gameId}
    /// </summary>
    public const string Button = "deletegameconfirm";

    public override string[] Permissions =>
    [
        UserPermission.BackOfficeGameList,
        UserPermission.BackOfficeGameDelete
    ];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public async Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        await dbContextProvider
            .Games
            .RemoveAsync(
                context.Base.Game,
                cancellationToken);

        dbContextProvider
            .AddPostCommitTask(async () =>
            {
                await context
                    .Base
                    .Client
                    .AnswerCallbackQuery(
                        context.Base.UpdateContext,
                        context.CallbackQueryId,
                        string
                            .Format(
                                TR.L + "_BACKOFFICE_GAME_DELETED_CLB",
                                context.Base.Game.Title.EscapeHtml()),
                        true,
                        cancellationToken: cancellationToken);

                await GameListBo.ResponseAsync(
                    context,
                    cancellationToken);
            });
    }

    protected override async Task<IDeleteGameConfirmBackOfficeContext?> PrepareContextAsync(
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
            var games = await dbContextProvider
                .Games
                .GetPaginatedAfterDeletionAsync(
                    game.Id,
                    gamePageNumber,
                    GameListBo.PageSize,
                    cancellationToken);

            var gamesCount = await dbContextProvider
                .Games
                .CountAsync(cancellationToken) - 1;

            var gamePageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(gamesCount) / GameListBo.PageSize));

            return IDeleteGameConfirmBackOfficeContext.Create(
                game,
                games,
                gamesCount,
                gamePageNumber,
                gamePageCount,
                backOfficeContext);
        }

        return null;
    }
}