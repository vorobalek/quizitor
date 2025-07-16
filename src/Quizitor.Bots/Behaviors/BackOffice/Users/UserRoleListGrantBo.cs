using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Users.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;

namespace Quizitor.Bots.Behaviors.BackOffice.Users;

internal sealed class UserRoleListGrantBo(
    IDbContextProvider dbContextProvider) :
    UserRoleListCommandBo(
        dbContextProvider)
{
    /// <summary>
    ///     userroles.{userId}.{userPageNumber}.{userRolePageNumber}.<b>grant</b>.{roleId}
    /// </summary>
    public const string Command = "grant";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public override string[] Permissions =>
    [
        UserPermission.BackOfficeUserView,
        UserPermission.BackOfficeUserEdit
    ];

    protected override string CommandInternal => Command;

    protected override async Task<bool> PerformCommandAsync(
        ICallbackQueryDataPrefixContext<IUserRoleListCommandBackOfficeContext> context,
        CancellationToken cancellationToken)
    {
        await _dbContextProvider
            .Roles
            .GrantByUserIdAsync(
                context.Base.User.Id,
                context.Base.Role,
                cancellationToken);

        _dbContextProvider
            .AddPostCommitTask(async () =>
                await context
                    .Base
                    .Client
                    .AnswerCallbackQuery(
                        context.Base.UpdateContext,
                        context.CallbackQueryId,
                        string.Format(
                            TR.L + "_BACKOFFICE_USER_ROLES_GRANTED_TXT",
                            context.Base.User.Id,
                            context.Base.User.FirstName.EscapeHtml(),
                            context.Base.User.LastName?.EscapeHtml(),
                            context.Base.Role.SystemName),
                        true,
                        cancellationToken: cancellationToken));

        return true;
    }
}