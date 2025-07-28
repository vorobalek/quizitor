using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;
using Game = Quizitor.Data.Entities.Game;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Infrastructure;

internal interface IGameListBackOfficeContext : IBackOfficeContext
{
    Game[] Games { get; }
    int GamesCount { get; }
    int GamePageNumber { get; }
    int GamePageCount { get; }

    static IGameListBackOfficeContext Create(
        Game[] games,
        int gamesCount,
        int gamePageNumber,
        int gamePageCount,
        IBackOfficeContext baseContext)
    {
        return new GameListBackOfficeContext(
            games,
            gamesCount,
            gamePageNumber,
            gamePageCount,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record GameListBackOfficeContext(
        Game[] Games,
        int GamesCount,
        int GamePageNumber,
        int GamePageCount,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IGameListBackOfficeContext;
}