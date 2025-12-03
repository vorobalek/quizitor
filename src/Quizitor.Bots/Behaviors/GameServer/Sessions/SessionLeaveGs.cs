using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Common;
using Quizitor.Data;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameServer.Sessions;

using Behavior = ICallbackQueryDataEqualsBehaviorTrait<IGameServerContext>;
using Context = ICallbackQueryDataEqualsContext<IGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class SessionLeaveGs(IDbContextProvider dbContextProvider) :
    GameServerBehavior(dbContextProvider),
    Behavior
{
    public const string Button = "sessionleave";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public override string[] Permissions => [];

    public string CallbackQueryDataValue => Button;

    public async Task PerformCallbackQueryDataEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        if (context.Base.SessionTeamInfo?.Leader?.Id == context.Base.Identity.User.Id)
        {
            await _dbContextProvider
                .Teams
                .UnsetLeaderAsync(
                    context.Base.SessionTeamInfo.Team.Id,
                    context.Base.Session.Id,
                    cancellationToken);
        }

        context.Base.Identity.User.SessionId = null;
        await _dbContextProvider
            .Users
            .UpdateAsync(
                context.Base.Identity.User,
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
                            TR.L + "_GAME_SERVER_LEAVE_SESSION_SUCCESS_TXT",
                            context.Base.Game.Title.Html,
                            context.Base.Session.Name.Html),
                        ParseMode.Html,
                        cancellationToken: cancellationToken);
                await GetSessionOrRequestQrCodeAsync(context.Base, cancellationToken);
            });
    }
}