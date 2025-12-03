using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Games.Sessions;
using Quizitor.Bots.Behaviors.GameAdmin.Sessions.Internal;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.QrCodeDataPrefix;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameAdmin.Sessions;

using QrCodeBehavior = IQrCodeDataPrefixBehaviorTrait<ISessionJoinGameServerContext>;
using QrCodeContext = IQrCodeDataPrefixContext<ISessionJoinGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class SessionJoinGa(
    IDbContextProvider dbContextProvider) :
    GameAdminBehaviorBase<ISessionJoinGameServerContext>(dbContextProvider),
    QrCodeBehavior
{
    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string[] GameAdminPermissions => [];

    public string QrCodeDataPrefixValue => $"{SessionGetQrBo.QrCommand}.";

    public async Task PerformQrCodeDataPrefixAsync(
        QrCodeContext context,
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

            var qrContext = IQrCodeDataPrefixContext.Create(
                IGameAdminContext.Create(
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
                            TR.L + "_GAME_ADMIN_SESSION_JOIN_TXT",
                            context.Base.ChosenGame.Title.Html,
                            context.Base.ChosenSession.Name.Html),
                        ParseMode.Html,
                        cancellationToken: cancellationToken);
                await MainPageGa.ResponseAsync(qrContext, cancellationToken);
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