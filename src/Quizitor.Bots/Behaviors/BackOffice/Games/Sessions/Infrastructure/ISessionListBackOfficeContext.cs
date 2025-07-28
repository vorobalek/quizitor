using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;
using Game = Quizitor.Data.Entities.Game;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Sessions.Infrastructure;

internal interface ISessionListBackOfficeContext : IBackOfficeContext
{
    Game Game { get; }
    Session[] Sessions { get; }
    int SessionsCount { get; }
    int GamePageNumber { get; }
    int SessionPageNumber { get; }
    int SessionPageCount { get; }

    static ISessionListBackOfficeContext Create(
        Game game,
        Session[] sessions,
        int sessionsCount,
        int gamePageNumber,
        int sessionPageNumber,
        int sessionPageCount,
        IBackOfficeContext baseContext)
    {
        return new SessionListBackOfficeContext(
            game,
            sessions,
            sessionsCount,
            gamePageNumber,
            sessionPageNumber,
            sessionPageCount,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record SessionListBackOfficeContext(
        Game Game,
        Session[] Sessions,
        int SessionsCount,
        int GamePageNumber,
        int SessionPageNumber,
        int SessionPageCount,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : ISessionListBackOfficeContext;
}