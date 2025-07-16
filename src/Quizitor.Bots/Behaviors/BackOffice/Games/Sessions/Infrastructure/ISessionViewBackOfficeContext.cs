using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;
using Game = Quizitor.Data.Entities.Game;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Sessions.Infrastructure;

internal interface ISessionViewBackOfficeContext : IBackOfficeContext
{
    Session Session { get; }
    Game Game { get; }
    int UsersCount { get; }
    int SubmissionsCount { get; }
    int GamePageNumber { get; }
    int SessionPageNumber { get; }

    static ISessionViewBackOfficeContext Create(
        Session session,
        Game game,
        int usersCount,
        int submissionsCount,
        int gamePageNumber,
        int sessionPageNumber,
        IBackOfficeContext baseContext)
    {
        return new SessionViewBackOfficeContext(
            session,
            game,
            usersCount,
            submissionsCount,
            gamePageNumber,
            sessionPageNumber,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record SessionViewBackOfficeContext(
        Session Session,
        Game Game,
        int UsersCount,
        int SubmissionsCount,
        int GamePageNumber,
        int SessionPageNumber,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : ISessionViewBackOfficeContext;
}