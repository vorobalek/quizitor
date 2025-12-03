using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors;

internal abstract class GameServerBehavior<TContext>(IDbContextProvider dbContextProvider) :
    GameServerBehaviorBase<TContext>(
        dbContextProvider)
    where TContext : IGameServerContext
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
                    TR.L + "_GAME_SERVER_REQUEST_QR_CODE_TXT",
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

        var teamInfo = await PrepareSessionTeamInfoAsync(
            _dbContextProvider,
            baseContext.Identity.User.Id,
            session.Id,
            cancellationToken);

        var context = IGameServerContext.Create(
            teamInfo,
            game,
            session,
            ILoadBalancerContext.Create(
                targetBot,
                baseContext));

        return await PrepareGameServerInternalAsync(context, cancellationToken);
    }

    public static async Task<ISessionTeamInfo?> PrepareSessionTeamInfoAsync(
        IDbContextProvider dbContextProvider,
        long userId,
        int sessionId,
        CancellationToken cancellationToken)
    {
        var team = await dbContextProvider
            .Teams
            .GetBySessionIdUserIdOrDefaultAsync(
                sessionId,
                userId,
                cancellationToken);

        var leader = team is not null
            ? await dbContextProvider
                .Users
                .GetLeaderByTeamIdSessionIdOrDefaultAsync(
                    team.Id,
                    sessionId,
                    cancellationToken)
            : null;

        var members = team is not null
            ? await dbContextProvider
                .Users
                .GetMembersByTeamIdSessionIdAsync(
                    team.Id,
                    sessionId,
                    cancellationToken)
            : [];

        var teamInfo = team is not null
            ? ISessionTeamInfo.Create(
                team,
                leader,
                [.. members.Where(x => x.SessionId == sessionId)],
                [.. members.Where(x => x.SessionId != sessionId)])
            : null;

        return teamInfo;
    }

    protected abstract Task<TContext?> PrepareGameServerInternalAsync(
        IGameServerContext gameServerContext,
        CancellationToken cancellationToken);
}

internal abstract class GameServerBehavior(
    IDbContextProvider dbContextProvider) :
    GameServerBehavior<IGameServerContext>(
        dbContextProvider)
{
    protected sealed override Task<IGameServerContext?> PrepareGameServerInternalAsync(
        IGameServerContext gameServerContext,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IGameServerContext?>(gameServerContext);
    }
}