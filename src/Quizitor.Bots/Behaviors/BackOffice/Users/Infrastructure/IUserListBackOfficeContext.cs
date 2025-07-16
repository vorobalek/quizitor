using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Users.Infrastructure;

internal interface IUserListBackOfficeContext : IBackOfficeContext
{
    User[] Users { get; }
    int UserPageNumber { get; }
    int UserPageCount { get; }

    static IUserListBackOfficeContext Create(
        User[] users,
        int userPageNumber,
        int userPageCount,
        IBackOfficeContext baseContext)
    {
        return new UserListBackOfficeContext(
            users,
            userPageNumber,
            userPageCount,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record UserListBackOfficeContext(
        User[] Users,
        int UserPageNumber,
        int UserPageCount,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IUserListBackOfficeContext;
}