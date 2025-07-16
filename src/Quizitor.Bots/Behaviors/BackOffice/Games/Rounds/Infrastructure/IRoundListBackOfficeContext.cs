using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;
using Game = Quizitor.Data.Entities.Game;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Rounds.Infrastructure;

internal interface IRoundListBackOfficeContext : IBackOfficeContext
{
    Game Game { get; }
    Round[] Rounds { get; }
    int GamePageNumber { get; }
    int RoundPageNumber { get; }
    int RoundPageCount { get; }

    static IRoundListBackOfficeContext Create(
        Game game,
        Round[] rounds,
        int gamePageNumber,
        int roundPageNumber,
        int roundPageCount,
        IBackOfficeContext baseContext)
    {
        return new RoundListBackOfficeContext(
            game,
            rounds,
            gamePageNumber,
            roundPageNumber,
            roundPageCount,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record RoundListBackOfficeContext(
        Game Game,
        Round[] Rounds,
        int GamePageNumber,
        int RoundPageNumber,
        int RoundPageCount,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IRoundListBackOfficeContext;
}