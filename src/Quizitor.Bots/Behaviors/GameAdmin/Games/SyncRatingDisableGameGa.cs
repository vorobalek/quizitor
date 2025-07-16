using LPlus;
using Quizitor.Bots.Behaviors.GameAdmin.Games.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Data;

namespace Quizitor.Bots.Behaviors.GameAdmin.Games;

using Context = ICallbackQueryDataEqualsContext<IGameGameAdminContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class SyncRatingDisableGameGa(
    IDbContextProvider dbContextProvider) :
    GameGa(dbContextProvider)
{
    public new const string Button = "syncratingdisable";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInternal => Button;

    public override async Task PerformCallbackQueryDataEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        context.Base.Session.SyncRating = false;
        await _dbContextProvider
            .Sessions
            .UpdateAsync(
                context.Base.Session,
                cancellationToken);
        _dbContextProvider.AddPostCommitTask(async () =>
        {
            await context
                .Base
                .Client
                .AnswerCallbackQuery(
                    context.Base.UpdateContext,
                    context.CallbackQueryId,
                    string.Format(
                        TR.L + "_GAME_ADMIN_SYNC_RATING_DISABLED_CLB",
                        context.Base.Game.Title,
                        context.Base.Session.Name),
                    true,
                    cancellationToken: cancellationToken);
            await base.PerformCallbackQueryDataEqualsAsync(context, cancellationToken);
        });
    }
}