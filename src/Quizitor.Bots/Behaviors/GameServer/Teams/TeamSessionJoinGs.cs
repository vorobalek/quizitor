using LPlus;
using Quizitor.Bots.Behaviors.GameServer.Teams.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.QrCodeDataPrefix;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameServer.Teams;

using Behavior = IQrCodeDataPrefixBehaviorTrait<ITeamSessionJoinGameServerContext>;
using Context = IQrCodeDataPrefixContext<ITeamSessionJoinGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class TeamSessionJoinGs(
    IDbContextProvider dbContextProvider) :
    GameServerBehaviorBase<ITeamSessionJoinGameServerContext>(dbContextProvider),
    Behavior
{
    /// <summary>
    ///     <b>teamjoin</b>.{teamId}.{sessionId}
    /// </summary>
    public const string QrCommand = "teamjoin";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public override string[] Permissions => [];

    public string QrCodeDataPrefixValue => $"{QrCommand}.";

    public async Task PerformQrCodeDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        if (context.Base.IsExpired)
        {
            await context
                .Base
                .Client
                .SendMessage(
                    context.Base.UpdateContext,
                    context.Base.TelegramUser.Id,
                    TR.L + "_SHARED_QR_CODE_IS_EXPIRED_TXT",
                    ParseMode.Html,
                    cancellationToken: cancellationToken);
        }
        else
        {
            context.Base.Identity.User.Session = context.Base.ChosenSession;
            await _dbContextProvider
                .Users
                .UpdateAsync(
                    context.Base.Identity.User,
                    cancellationToken);

            var currentTeamInfo = await GameServerBehavior.PrepareSessionTeamInfoAsync(
                _dbContextProvider,
                context.Base.Identity.User.Id,
                context.Base.ChosenSession.Id,
                cancellationToken);

            if (currentTeamInfo is not null &&
                currentTeamInfo.Team.Id != context.Base.ChosenTeam.Id)
            {
                await _dbContextProvider
                    .Teams
                    .LeaveAsync(
                        currentTeamInfo.Team.Id,
                        context.Base.ChosenSession.Id,
                        context.Base.Identity.User.Id,
                        cancellationToken);
            }

            await _dbContextProvider
                .Teams
                .AddMemberAsync(context.Base.Identity.User.Id,
                    context.Base.ChosenSession.Id, context.Base.ChosenTeam.Id, cancellationToken);

            var teamInfo = await GameServerBehavior.PrepareSessionTeamInfoAsync(
                _dbContextProvider,
                context.Base.Identity.User.Id,
                context.Base.ChosenSession.Id,
                cancellationToken);

            var qrContext = IQrCodeDataPrefixContext.Create(
                IGameServerContext.Create(
                    teamInfo,
                    context.Base.ChosenGame,
                    context.Base.ChosenSession,
                    context.Base),
                QrCodeDataPrefixValue);

            _dbContextProvider.AddPostCommitTask(async () =>
            {
                await context
                    .Base
                    .Client
                    .SendMessage(
                        context.Base.UpdateContext,
                        context.Base.TelegramUser.Id,
                        string.Format(
                            TR.L + "_GAME_SERVER_TEAM_JOIN_TXT",
                            context.Base.ChosenGame.Title.EscapeHtml(),
                            context.Base.ChosenSession.Name.EscapeHtml(),
                            context.Base.ChosenTeam.Name.EscapeHtml()),
                        ParseMode.Html,
                        cancellationToken: cancellationToken);
                if (qrContext is not null)
                    await MainPageGs.ResponseAsync(qrContext.Base, null, cancellationToken);
            });
        }
    }

    protected override async Task<ITeamSessionJoinGameServerContext?> PrepareLoadBalancerInternalAsync(
        Bot? targetBot,
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        var qrContext = IQrCodeDataPrefixContext.Create(baseContext, QrCodeDataPrefixValue);
        if (qrContext?.QrCodeDataPostfix.Split('.') is
            [
                {
                } teamIdString,
                {
                } sessionIdString,
                {
                } codeExpiryString
            ] &&
            int.TryParse(sessionIdString, out var sessionId) &&
            int.TryParse(teamIdString, out var teamId) &&
            long.TryParse(codeExpiryString, out var codeExpiry) &&
            await _dbContextProvider.Sessions.GetByIdOrDefaultAsync(sessionId, cancellationToken) is { } session &&
            await _dbContextProvider.Teams.GetByIdOrDefaultAsync(teamId, cancellationToken) is { } team)
        {
            var game = await _dbContextProvider
                .Games
                .GetByIdAsync(
                    session.GameId,
                    cancellationToken);

            var serverTime = await _dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);
            var isExpired = codeExpiry != 0 && serverTime > DateTimeOffset.FromUnixTimeSeconds(codeExpiry);

            return ITeamSessionJoinGameServerContext.Create(
                session,
                team,
                game,
                isExpired,
                ILoadBalancerContext.Create(
                    targetBot,
                    baseContext));
        }

        return null;
    }
}