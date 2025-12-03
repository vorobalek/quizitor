using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Games.Sessions;
using Quizitor.Bots.Behaviors.GameServer.Sessions.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.QrCodeDataPrefix;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameServer.Sessions;

using Behavior = IQrCodeDataPrefixBehaviorTrait<ISessionJoinGameServerContext>;
using Context = IQrCodeDataPrefixContext<ISessionJoinGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class SessionJoinGs(
    IDbContextProvider dbContextProvider) :
    GameServerBehaviorBase<ISessionJoinGameServerContext>(dbContextProvider),
    Behavior
{
    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public override string[] Permissions => [];

    public string QrCodeDataPrefixValue => $"{SessionGetQrBo.QrCommand}.";

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
                            TR.L + "_GAME_SERVER_SESSION_JOIN_TXT",
                            context.Base.ChosenGame.Title.Html,
                            context.Base.ChosenSession.Name.Html),
                        ParseMode.Html,
                        cancellationToken: cancellationToken);
                if (qrContext is not null)
                    await MainPageGs.ResponseAsync(qrContext.Base, null, cancellationToken);
            });
        }
    }

    protected override async Task<ISessionJoinGameServerContext?> PrepareLoadBalancerInternalAsync(
        Bot? targetBot,
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        var qrContext = IQrCodeDataPrefixContext.Create(baseContext, QrCodeDataPrefixValue);
        if (qrContext?.QrCodeDataPostfix.Split('.') is
            [
                {
                } sessionIdString,
                {
                } codeExpiryString
            ] &&
            int.TryParse(sessionIdString, out var sessionId) &&
            long.TryParse(codeExpiryString, out var codeExpiry) &&
            await _dbContextProvider.Sessions.GetByIdOrDefaultAsync(sessionId, cancellationToken) is { } session)
        {
            var game = await _dbContextProvider
                .Games
                .GetByIdAsync(
                    session.GameId,
                    cancellationToken);

            var serverTime = await _dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);
            var isExpired = codeExpiry != 0 && serverTime > DateTimeOffset.FromUnixTimeSeconds(codeExpiry);

            return ISessionJoinGameServerContext.Create(
                session,
                game,
                isExpired,
                ILoadBalancerContext.Create(
                    targetBot,
                    baseContext));
        }

        return null;
    }
}