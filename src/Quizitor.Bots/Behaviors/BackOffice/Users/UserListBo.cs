using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Users.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Users;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IUserListBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<IUserListBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal class UserListBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<IUserListBackOfficeContext>,
    Behavior
{
    private const int PageSize = 10;

    /// <summary>
    ///     <b>users</b>.{userPageNumber}
    /// </summary>
    public const string Button = "users";

    public override string[] Permissions => [UserPermission.BackOfficeUserList];

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
                string.Format(
                    TR.L + "_BACKOFFICE_USERS_TXT",
                    context.Base.UsersCount),
                ParseMode.Html,
                replyMarkup: Keyboards.UserList(
                    context.Base.Users,
                    context.Base.UserPageNumber,
                    context.Base.UserPageCount), cancellationToken: cancellationToken);
    }

    protected override async Task<IUserListBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } userPageNumberString
            ] &&
            int.TryParse(userPageNumberString, out var userPageNumber))
        {
            var users = await dbContextProvider
                .Users
                .GetPaginatedAsync(
                    userPageNumber,
                    PageSize,
                    cancellationToken);

            var usersCount = await dbContextProvider
                .Users
                .CountAsync(cancellationToken);

            var userPageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(usersCount) / PageSize));

            return IUserListBackOfficeContext.Create(
                users,
                usersCount,
                userPageNumber,
                userPageCount,
                backOfficeContext);
        }

        return null;
    }
}