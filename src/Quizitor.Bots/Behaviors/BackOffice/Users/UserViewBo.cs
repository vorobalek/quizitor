using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Users.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Configuration;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Users;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IUserViewBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<IUserViewBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class UserViewBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<IUserViewBackOfficeContext>,
    Behavior
{
    /// <summary>
    ///     <b>user</b>.{userId}.{userPageNumber}
    /// </summary>
    public const string Button = "user";

    public override string[] Permissions => [UserPermission.BackOfficeUserView];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return context
            .Base
            .Client
            .EditMessageText(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                context.MessageId,
                string
                    .Format(
                        TR.L + "_BACKOFFICE_USER_VIEW_TXT",
                        context.Base.User.Id,
                        context.Base.User.FirstName.Html,
                        context.Base.User.LastName is null
                            ? TR.L + "_SHARED_NO_TXT"
                            : context.Base.User.LastName.Html,
                        context.Base.User.Username is null
                            ? TR.L + "_SHARED_NO_TXT"
                            : string.Format(
                                TR.L + "_BACKOFFICE_USER_VIEW_LINK_TXT",
                                context.Base.User.Username.Html
                            ),
                        context.Base.GameServer is null
                            ? TR.L + "_SHARED_NO_TXT"
                            : string.Format(
                                TR.L + "_BACKOFFICE_USER_VIEW_BOT_INFO_TXT",
                                context.Base.GameServer.Name.Html,
                                context.Base.GameServer.IsActive
                                    ? TR.L + "_BACKOFFICE_BOT_ACTIVE_TXT"
                                    : TR.L + "_BACKOFFICE_BOT_INACTIVE_TXT",
                                context.Base.GameServer.Username?.Html
                            ),
                        context.Base.GameAdmin is null
                            ? TR.L + "_SHARED_NO_TXT"
                            : string.Format(
                                TR.L + "_BACKOFFICE_USER_VIEW_BOT_INFO_TXT",
                                context.Base.GameAdmin.Name.Html,
                                context.Base.GameAdmin.IsActive
                                    ? TR.L + "_BACKOFFICE_BOT_ACTIVE_TXT"
                                    : TR.L + "_BACKOFFICE_BOT_INACTIVE_TXT",
                                context.Base.GameAdmin.Username?.Html
                            ),
                        context.Base.SubmissionsCount,
                        context.Base.SessionsCount,
                        string.Join(
                            Environment.NewLine,
                            context.Base.LastInteractions
                                .OrderByDescending(x => x.LastInteraction)
                                .Select(x => string.Format(
                                    TR.L + "_BACKOFFICE_USER_BOT_INTERACTION_VIEW_TXT",
                                    x.BotUsername,
                                    TR.L + x.LastInteraction + "dd.MM.yyyy HH:mm:ss tt zz"))),
                        context.Base.HasFullAccess
                            ? TR.L + "_SHARED_YES_TXT"
                            : TR.L + "_SHARED_NO_TXT",
                        string.Join(
                            Environment.NewLine,
                            context.Base.PermissionOrigins
                                .OrderBy(x => x.Key)
                                .Select(kv => string.Format(
                                    TR.L + "_BACKOFFICE_USER_PERMISSION_VIEW_TXT",
                                    kv.Key,
                                    string.Join(
                                        ", ",
                                        kv.Value.Select(source =>
                                            source is null
                                                ? TR.L + "_BACKOFFICE_USER_PERMISSION_EXPLICIT_VIEW_TXT"
                                                : string.Format(
                                                    TR.L + "_BACKOFFICE_USER_PERMISSION_SOURCE_VIEW_TXT",
                                                    source
                                                )
                                        )
                                    )
                                ))
                        )
                    ),
                ParseMode.Html,
                replyMarkup: Keyboards.UserView(
                    context.Base.User.Id,
                    context.Base.UserPageNumber), cancellationToken: cancellationToken);
    }

    protected override async Task<IUserViewBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } userIdString,
                {
                } userPageNumberString
            ] &&
            long.TryParse(userIdString, out var userId) &&
            int.TryParse(userPageNumberString, out var userPageNumber) &&
            await dbContextProvider.Users.GetByIdOrDefaultAsync(userId, cancellationToken) is { } user)
        {
            var gameServer = await dbContextProvider
                .Bots
                .GetByIdOrDefaultAsync(
                    user.GameServerId,
                    cancellationToken);

            var gameAdmin = await dbContextProvider
                .Bots
                .GetByIdOrDefaultAsync(
                    user.GameAdminId,
                    cancellationToken);

            var submissionsCount = await dbContextProvider
                .Submissions
                .CountByUserIdAsync(
                    user.Id,
                    cancellationToken);

            var sessionsCount = await dbContextProvider
                .Sessions
                .CountByUserIdAsync(
                    user.Id,
                    cancellationToken);

            var lastInteractions = await dbContextProvider
                .BotInteractions
                .GetByUserIdAsync(
                    user.Id,
                    cancellationToken);

            var hasFullAccess =
                !TelegramBotConfiguration.IsSaUserAuthorizationEnabled ||
                TelegramBotConfiguration.AuthorizedUserIds.Contains(user.Id);

            var permissionOrigins = (await dbContextProvider
                    .Users
                    .GetPermissionsByUserIdAsync(
                        user.Id,
                        cancellationToken))
                .Select(x => x.SystemName)
                .ToDictionary(
                    x => x,
                    _ => new List<string?>([null]));

            var roles = await dbContextProvider
                .Roles
                .GetByUserIdAsync(
                    user.Id,
                    cancellationToken);

            foreach (var role in roles)
            {
                var rolePermissions = await dbContextProvider
                    .Roles
                    .GetPermissionsByRoleIdAsync(
                        role.Id,
                        cancellationToken);

                foreach (var permission in rolePermissions)
                {
                    if (!permissionOrigins.ContainsKey(permission.SystemName))
                    {
                        permissionOrigins.Add(permission.SystemName, []);
                    }

                    permissionOrigins[permission.SystemName].Add(role.SystemName);
                }
            }

            return IUserViewBackOfficeContext.Create(
                user,
                userPageNumber,
                gameServer,
                gameAdmin,
                submissionsCount,
                sessionsCount,
                hasFullAccess,
                lastInteractions,
                permissionOrigins,
                backOfficeContext);
        }

        return null;
    }
}