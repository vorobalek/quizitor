using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Data;

namespace Quizitor.Bots.Behaviors.GameServer.Teams;

using Context = ICallbackQueryDataEqualsContext<IGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class TeamUnsetLeaderGs(IDbContextProvider dbContextProvider) : TeamGs(dbContextProvider)
{
    public new const string Button = "teamunsetleader";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInternal => Button;

    public override async Task PerformCallbackQueryDataEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        if (context.Base.SessionTeamInfo is not { } teamInfo ||
            teamInfo.Leader?.Id != context.Base.Identity.User.Id)
        {
            await base.PerformCallbackQueryDataEqualsAsync(context, cancellationToken);
            return;
        }

        await _dbContextProvider
            .Teams
            .UnsetLeaderAsync(
                teamInfo.Team.Id,
                context.Base.Session.Id,
                cancellationToken);

        _dbContextProvider.AddPostCommitTask(async () =>
            await ResponseAsync(
                IGameServerContext.Create(
                    ISessionTeamInfo.Create(
                        teamInfo.Team,
                        null,
                        teamInfo.Members,
                        teamInfo.OfflineMembers),
                    context.Base.Game,
                    context.Base.Session,
                    context.Base),
                context.MessageId,
                cancellationToken));
    }
}