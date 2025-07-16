using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;
using Game = Quizitor.Data.Entities.Game;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Rounds.Infrastructure;

internal interface IRoundViewBackOfficeContext : IBackOfficeContext
{
    Round Round { get; }
    Game Game { get; }
    Question[] Questions { get; }
    int GamePageNumber { get; }
    int RoundPageNumber { get; }
    int QuestionPageNumber { get; }
    int QuestionPageCount { get; }

    static IRoundViewBackOfficeContext Create(
        Round round,
        Game game,
        Question[] questions,
        int gamePageNumber,
        int roundPageNumber,
        int questionPageNumber,
        int questionPageCount,
        IBackOfficeContext baseContext)
    {
        return new RoundViewBackOfficeContext(
            round,
            game,
            questions,
            gamePageNumber,
            roundPageNumber,
            questionPageNumber,
            questionPageCount,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record RoundViewBackOfficeContext(
        Round Round,
        Game Game,
        Question[] Questions,
        int GamePageNumber,
        int RoundPageNumber,
        int QuestionPageNumber,
        int QuestionPageCount,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IRoundViewBackOfficeContext;
}