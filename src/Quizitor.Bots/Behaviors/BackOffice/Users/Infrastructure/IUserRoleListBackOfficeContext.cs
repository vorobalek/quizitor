using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Users.Infrastructure;

internal interface IUserRoleListBackOfficeContext : IBackOfficeContext
{
    User User { get; }
    Role[] Roles { get; }
    Role[] UserRoles { get; }
    int UserPageNumber { get; }
    int UserRolePageNumber { get; }
    int PageCount { get; }

    static IUserRoleListBackOfficeContext Create(
        User user,
        Role[] roles,
        Role[] userRoles,
        int userPageNumber,
        int userRolePageNumber,
        int pageCount,
        IBackOfficeContext baseContext)
    {
        return new UserRoleListBackOfficeContext(
            user,
            roles,
            userRoles,
            userPageNumber,
            userRolePageNumber,
            pageCount,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record UserRoleListBackOfficeContext(
        User User,
        Role[] Roles,
        Role[] UserRoles,
        int UserPageNumber,
        int UserRolePageNumber,
        int PageCount,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IUserRoleListBackOfficeContext;
}