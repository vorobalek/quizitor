using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Data;

namespace Quizitor.Bots.Behaviors.BackOffice.Users.Infrastructure;

using Context = ICallbackQueryDataPrefixContext<IUserRoleListCommandBackOfficeContext>;

internal abstract class UserRoleListCommandBo(
    IDbContextProvider dbContextProvider) :
    UserRoleListBo<IUserRoleListCommandBackOfficeContext>(
        dbContextProvider)
{
    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected abstract string CommandInternal { get; }

    public override bool ShouldPerformCallbackQueryDataPrefix(IBehaviorContext baseContext)
    {
        return base.ShouldPerformCallbackQueryDataPrefix(baseContext) &&
               baseContext.UpdateContext.Update.CallbackQuery!.Data!.Split('.') is { Length: > 4 } args &&
               args[4] == CommandInternal;
    }

    protected override async Task<IUserRoleListCommandBackOfficeContext?> PrepareContextAsync(
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
                } userRolesPageNumberString,
                {
                } command,
                {
                } roleIdString
            ] &&
            command == CommandInternal &&
            long.TryParse(userIdString, out var userId) &&
            int.TryParse(userPageNumberString, out var userPageNumber) &&
            int.TryParse(userRolesPageNumberString, out var userRolesPageNumber) &&
            int.TryParse(roleIdString, out var roleId) &&
            await _dbContextProvider.Users.GetByIdOrDefaultAsync(userId, cancellationToken) is { } user &&
            await _dbContextProvider.Roles.GetByIdOrDefaultAsync(roleId, cancellationToken) is { } role)
        {
            var userRoleListContext = await PrepareBaseAsync(
                backOfficeContext,
                user,
                userPageNumber,
                userRolesPageNumber,
                cancellationToken);
            return IUserRoleListCommandBackOfficeContext.Create(
                role,
                userRoleListContext);
        }

        return null;
    }

    public override async Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        if (await PerformCommandAsync(context, cancellationToken))
        {
            var newContext = ICallbackQueryDataPrefixContext.Create(
                (await PrepareAsync(context.Base, cancellationToken))!,
                CallbackQueryDataPrefixValue);
            _dbContextProvider
                .AddPostCommitTask(async () =>
                    await ResponseAsync(newContext!, cancellationToken));
        }
        else
        {
            await ResponseAsync(context, cancellationToken);
        }
    }

    protected abstract Task<bool> PerformCommandAsync(
        Context context,
        CancellationToken cancellationToken);
}