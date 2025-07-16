using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Users.Infrastructure;

internal interface IUserRoleListCommandBackOfficeContext : IUserRoleListBackOfficeContext
{
    Role Role { get; }

    static IUserRoleListCommandBackOfficeContext Create(
        Role role,
        IUserRoleListBackOfficeContext baseContext)
    {
        return new UserRoleListCommandBackOfficeContext(
            role,
            baseContext.User,
            baseContext.Roles,
            baseContext.UserRoles,
            baseContext.UserPageNumber,
            baseContext.UserRolePageNumber,
            baseContext.PageCount,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record UserRoleListCommandBackOfficeContext(
        Role Role,
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
        IIdentity Identity) : IUserRoleListCommandBackOfficeContext;
}