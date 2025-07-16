using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;
using Game = Quizitor.Data.Entities.Game;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Rounds.Questions.Infrastructure;

internal interface IQuestionBackOfficeContext : IBackOfficeContext
{
    Question Question { get; }
    Round Round { get; }
    Game Game { get; }
    QuestionOption[] Options { get; }
    QuestionRule[] Rules { get; }
    int QuestionPageNumber { get; }
    int RoundPageNumber { get; }
    int GamePageNumber { get; }

    static IQuestionBackOfficeContext Create(
        Question question,
        Round round,
        Game game,
        QuestionOption[] options,
        QuestionRule[] rules,
        int gamePageNumber,
        int roundPageNumber,
        int questionPageNumber,
        IBackOfficeContext baseContext)
    {
        return new QuestionBackOfficeContext(
            question,
            round,
            game,
            options,
            rules,
            gamePageNumber,
            roundPageNumber,
            questionPageNumber,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record QuestionBackOfficeContext(
        Question Question,
        Round Round,
        Game Game,
        QuestionOption[] Options,
        QuestionRule[] Rules,
        int GamePageNumber,
        int RoundPageNumber,
        int QuestionPageNumber,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IQuestionBackOfficeContext;
}