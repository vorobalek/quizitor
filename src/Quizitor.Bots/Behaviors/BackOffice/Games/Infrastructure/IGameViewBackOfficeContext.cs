using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;
using Game = Quizitor.Data.Entities.Game;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Infrastructure;

internal interface IGameViewBackOfficeContext : IBackOfficeContext
{
    Game Game { get; }
    int GamePageNumber { get; }
    int RoundsCount { get; }
    int SessionsCount { get; }

    static IGameViewBackOfficeContext Create(
        Game game,
        int gamePageNumber,
        int roundsCount,
        int sessionsCount,
        IBackOfficeContext baseContext)
    {
        return new GameViewBackOfficeContext(
            game,
            gamePageNumber,
            roundsCount,
            sessionsCount,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record GameViewBackOfficeContext(
        Game Game,
        int GamePageNumber,
        int RoundsCount,
        int SessionsCount,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IGameViewBackOfficeContext;
}