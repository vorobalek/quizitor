using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors;

internal abstract class GameAdminBehavior<TContext>(IDbContextProvider dbContextProvider) :
    GameAdminBehaviorBase<TContext>(dbContextProvider)
    where TContext : IGameAdminContext
{
    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected async Task<Session?> GetSessionOrRequestQrCodeAsync(
        IBehaviorContext context,
        CancellationToken cancellationToken)
    {
        if (await _dbContextProvider
                .Sessions
                .GetByIdOrDefaultAsync(
                    context.Identity.User.SessionId,
                    cancellationToken) is not { } session)
        {
            await context
                .Client
                .SendMessage(
                    context.UpdateContext,
                    context.TelegramUser.Id,
                    TR.L + "_GAME_ADMIN_REQUEST_QR_CODE_TXT",
                    ParseMode.Html,
                    cancellationToken: cancellationToken);
            return null;
        }

        return session;
    }

    protected sealed override async Task<TContext?> PrepareLoadBalancerInternalAsync(
        Bot? targetBot,
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        if (await GetSessionOrRequestQrCodeAsync(baseContext, cancellationToken) is not { } session)
            return default;

        var game = await _dbContextProvider
            .Games
            .GetByIdAsync(
                session.GameId,
                cancellationToken);

        var context = IGameAdminContext.Create(
            game,
            session,
            ILoadBalancerContext.Create(
                targetBot,
                baseContext));

        return await PrepareGameAdminInternalAsync(context, cancellationToken);
    }

    protected abstract Task<TContext?> PrepareGameAdminInternalAsync(
        IGameAdminContext gameAdminContext,
        CancellationToken cancellationToken);
}

internal abstract class GameAdminBehavior(IDbContextProvider dbContextProvider) :
    GameAdminBehavior<IGameAdminContext>(dbContextProvider)
{
    protected sealed override Task<IGameAdminContext?> PrepareGameAdminInternalAsync(
        IGameAdminContext gameAdminContext,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IGameAdminContext?>(gameAdminContext);
    }
}