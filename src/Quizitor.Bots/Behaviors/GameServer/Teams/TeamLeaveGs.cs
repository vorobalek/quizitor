using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Common;
using Quizitor.Data;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameServer.Teams;

using Context = ICallbackQueryDataEqualsContext<IGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class TeamLeaveGs(
    IDbContextProvider dbContextProvider) :
    TeamGs(dbContextProvider)
{
    public new const string Button = "teamleave";

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
            .LeaveAsync(
                teamInfo.Team.Id,
                context.Base.Session.Id,
                context.Base.Identity.User.Id,
                cancellationToken);

        _dbContextProvider
            .AddPostCommitTask(async () =>
            {
                await context
                    .Base
                    .Client
                    .EditMessageText(
                        context.Base.UpdateContext,
                        context.Base.TelegramUser.Id,
                        context.MessageId,
                        string.Format(
                            TR.L + "_GAME_SERVER_TEAM_LEAVE_SUCCESS_TXT",
                            teamInfo.Team.Name.EscapeHtml()),
                        ParseMode.Html,
                        cancellationToken: cancellationToken);
                await MainPageGs.ResponseAsync(
                    IGameServerContext.Create(
                        null,
                        context.Base.Game,
                        context.Base.Session,
                        context.Base),
                    null,
                    cancellationToken);
            });
    }
}