using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Users.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Users;

using Context = ICallbackQueryDataPrefixContext<IUserRoleListBackOfficeContext>;

internal abstract class UserRoleListBo<TContext>(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<TContext>,
    ICallbackQueryDataPrefixBehaviorTrait<TContext>
    where TContext : IUserRoleListBackOfficeContext
{
    /// <summary>
    ///     <b>userroles</b>.{userId}.{userPageNumber}.{userRolePageNumber}<br />
    ///     <b>userroles</b>.{userId}.{userPageNumber}.{userRolePageNumber}.[grant|revoke].{roleId}
    /// </summary>
    public const string Button = "userroles";

    protected const int PageSize = 10;

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public abstract Task PerformCallbackQueryDataPrefixAsync(
        ICallbackQueryDataPrefixContext<TContext> context,
        CancellationToken cancellationToken);

    protected async Task<IUserRoleListBackOfficeContext> PrepareBaseAsync(
        IBackOfficeContext backOfficeContext,
        User user,
        int userPageNumber,
        int userRolesPageNumber,
        CancellationToken cancellationToken)
    {
        var roles = await dbContextProvider
            .Roles
            .GetPaginatedAsync(
                userRolesPageNumber,
                PageSize,
                cancellationToken);

        var pageCount = Convert.ToInt32(
            Math.Ceiling(
                Convert.ToDouble(
                    await dbContextProvider
                        .Roles
                        .CountAsync(cancellationToken)) / PageSize));

        var userRoles = await dbContextProvider
            .Roles
            .GetByUserIdAsync(
                user.Id,
                cancellationToken);

        return IUserRoleListBackOfficeContext.Create(
            user,
            roles,
            userRoles,
            userPageNumber,
            userRolesPageNumber,
            pageCount,
            backOfficeContext);
    }

    protected Task ResponseAsync(
        ICallbackQueryDataPrefixContext<TContext> context,
        CancellationToken cancellationToken)
    {
        return context
            .Base
            .Client
            .EditMessageText(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                context.MessageId,
                string.Format(
                    TR.L + "_BACKOFFICE_USER_ROLES_TXT",
                    context.Base.User.Id,
                    context.Base.User.FirstName.EscapeHtml(),
                    context.Base.User.LastName?.EscapeHtml()),
                ParseMode.Html,
                replyMarkup: Keyboards.UserRoles(
                    context.Base.Roles,
                    context.Base.UserRoles,
                    context.Base.User.Id,
                    context.Base.UserPageNumber,
                    context.Base.UserRolePageNumber,
                    context.Base.PageCount),
                cancellationToken: cancellationToken);
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class UserRoleListBo(
    IDbContextProvider dbContextProvider) :
    UserRoleListBo<IUserRoleListBackOfficeContext>(
        dbContextProvider)
{
    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public override string[] Permissions => [UserPermission.BackOfficeUserView];

    protected override async Task<IUserRoleListBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } userIdString,
                {
                } userPageNumberString,
                {
                } userRolesPageNumberString
            ] &&
            long.TryParse(userIdString, out var userId) &&
            int.TryParse(userPageNumberString, out var userPageNumber) &&
            int.TryParse(userRolesPageNumberString, out var userRolesPageNumber) &&
            await _dbContextProvider.Users.GetByIdOrDefaultAsync(userId, cancellationToken) is { } user)
        {
            return await PrepareBaseAsync(
                backOfficeContext,
                user,
                userPageNumber,
                userRolesPageNumber,
                cancellationToken);
        }

        return null;
    }

    public override Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context, cancellationToken);
    }
}