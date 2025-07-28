using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Users.Infrastructure;

internal interface IUserViewBackOfficeContext : IBackOfficeContext
{
    User User { get; }
    int UserPageNumber { get; }
    Bot? GameServer { get; }
    Bot? GameAdmin { get; }
    int SubmissionsCount { get; }
    int SessionsCount { get; }
    bool HasFullAccess { get; }
    BotInteraction[] LastInteractions { get; }
    Dictionary<string, List<string?>> PermissionOrigins { get; }

    static IUserViewBackOfficeContext Create(
        User user,
        int userPageNumber,
        Bot? gameServer,
        Bot? gameAdmin,
        int submissionsCount,
        int sessionsCount,
        bool hasFullAccess,
        BotInteraction[] lastInteractions,
        Dictionary<string, List<string?>> permissionOrigins,
        IBackOfficeContext baseContext)
    {
        return new UserViewBackOfficeContext(
            user,
            userPageNumber,
            gameServer,
            gameAdmin,
            submissionsCount,
            sessionsCount,
            hasFullAccess,
            lastInteractions,
            permissionOrigins,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record UserViewBackOfficeContext(
        User User,
        int UserPageNumber,
        Bot? GameServer,
        Bot? GameAdmin,
        int SubmissionsCount,
        int SessionsCount,
        bool HasFullAccess,
        BotInteraction[] LastInteractions,
        Dictionary<string, List<string?>> PermissionOrigins,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IUserViewBackOfficeContext;
}