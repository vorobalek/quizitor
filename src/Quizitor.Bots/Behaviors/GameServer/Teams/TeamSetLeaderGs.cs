using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Data;

namespace Quizitor.Bots.Behaviors.GameServer.Teams;

using Context = ICallbackQueryDataEqualsContext<IGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class TeamSetLeaderGs(IDbContextProvider dbContextProvider) : TeamGs(dbContextProvider)
{
    public new const string Button = "teamsetleader";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInternal => Button;

    public override async Task PerformCallbackQueryDataEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        if (context.Base.SessionTeamInfo is not { } teamInfo)
        {
            await base.PerformCallbackQueryDataEqualsAsync(context, cancellationToken);
            return;
        }

        await _dbContextProvider
            .Teams
            .SetLeaderAsync(
                teamInfo.Team.Id,
                context.Base.Session.Id,
                context.Base.Identity.User.Id,
                cancellationToken);

        _dbContextProvider.AddPostCommitTask(async () =>
            await ResponseAsync(
                IGameServerContext.Create(
                    ISessionTeamInfo.Create(
                        teamInfo.Team,
                        context.Base.Identity.User,
                        teamInfo.Members,
                        teamInfo.OfflineMembers),
                    context.Base.Game,
                    context.Base.Session,
                    context.Base),
                context.MessageId,
                cancellationToken));
    }
}