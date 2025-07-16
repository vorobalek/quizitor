using LPlus;
using Quizitor.Bots.Behaviors.GameServer.Teams.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Common;
using Quizitor.Data;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameServer.Teams;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<ITeamJoinGameServerContext>;
using Context = ICallbackQueryDataPrefixContext<ITeamJoinGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class TeamJoinGs(
    IDbContextProvider dbContextProvider) :
    GameServerBehavior<ITeamJoinGameServerContext>(dbContextProvider),
    Behavior
{
    /// <summary>
    ///     <b>teamjoin</b>.{teamId}
    /// </summary>
    public const string Button = "teamjoin";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public override string[] Permissions => [];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public async Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        if (context.Base.SessionTeamInfo is { } currentTeamInfo &&
            currentTeamInfo.Team.Id != context.Base.Team.Id)
        {
            await _dbContextProvider
                .Teams
                .LeaveAsync(
                    currentTeamInfo.Team.Id,
                    context.Base.Session.Id,
                    context.Base.Identity.User.Id,
                    cancellationToken);
        }

        await _dbContextProvider
            .Teams
            .AddMemberAsync(context.Base.Identity.User.Id,
                context.Base.Session.Id, context.Base.Team.Id, cancellationToken);

        var newTeamInfo = await GameServerBehavior.PrepareSessionTeamInfoAsync(
            _dbContextProvider,
            context.Base.Identity.User.Id,
            context.Base.Session.Id,
            cancellationToken);

        var newContext = IGameServerContext.Create(
            newTeamInfo,
            context.Base.Game,
            context.Base.Session,
            context.Base);

        _dbContextProvider.AddPostCommitTask(async () =>
        {
            await context
                .Base
                .Client
                .EditMessageText(
                    context.Base.UpdateContext,
                    context.Base.TelegramUser.Id,
                    context.MessageId,
                    string.Format(
                        TR.L + "_GAME_SERVER_TEAM_JOIN_TXT",
                        context.Base.Game.Title.EscapeHtml(),
                        context.Base.Session.Name.EscapeHtml(),
                        context.Base.Team.Name.EscapeHtml()),
                    ParseMode.Html,
                    cancellationToken: cancellationToken);
            await MainPageGs.ResponseAsync(newContext, null, cancellationToken);
        });
    }

    protected override async Task<ITeamJoinGameServerContext?> PrepareGameServerInternalAsync(
        IGameServerContext gameServerContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(
            gameServerContext,
            CallbackQueryDataPrefixValue);

        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } teamIdString
            ] &&
            int.TryParse(teamIdString, out var teamId) &&
            await _dbContextProvider.Teams.GetByIdOrDefaultAsync(teamId, cancellationToken) is { } team &&
            team.OwnerId == gameServerContext.Identity.User.Id)
        {
            return ITeamJoinGameServerContext.Create(
                team,
                gameServerContext);
        }

        return null;
    }
}