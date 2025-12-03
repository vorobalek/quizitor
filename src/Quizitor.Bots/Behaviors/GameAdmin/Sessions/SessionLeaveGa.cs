using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameAdmin.Sessions;

using Behavior = ICallbackQueryDataEqualsBehaviorTrait<IGameAdminContext>;
using Context = ICallbackQueryDataEqualsContext<IGameAdminContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class SessionLeaveGa(IDbContextProvider dbContextProvider) :
    GameAdminBehavior(dbContextProvider),
    Behavior
{
    public const string Button = "sessionleave";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string[] GameAdminPermissions => [UserPermission.GameAdminSessionLeave];
    public string CallbackQueryDataValue => Button;

    public async Task PerformCallbackQueryDataEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        var team = await _dbContextProvider
            .Teams
            .GetBySessionIdUserIdOrDefaultAsync(
                context.Base.Session.Id,
                context.Base.Identity.User.Id,
                cancellationToken);

        if (team is not null)
        {
            var leader = await _dbContextProvider
                .Users
                .GetLeaderByTeamIdSessionIdOrDefaultAsync(
                    team.Id,
                    context.Base.Session.Id,
                    cancellationToken);
            if (leader?.Id == context.Base.Identity.User.Id)
            {
                await _dbContextProvider
                    .Teams
                    .UnsetLeaderAsync(
                        team.Id,
                        context.Base.Session.Id,
                        cancellationToken);
            }
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
                            TR.L + "_GAME_ADMIN_LEAVE_SESSION_SUCCESS_TXT",
                            context.Base.Game.Title.Html,
                            context.Base.Session.Name.Html),
                        ParseMode.Html,
                        cancellationToken: cancellationToken);
                await GetSessionOrRequestQrCodeAsync(context.Base, cancellationToken);
            });
    }
}