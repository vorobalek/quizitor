using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions.Infrastructure;

internal interface IQuestionGameAdminContext : IGameAdminContext
{
    Round Round { get; }
    Question Question { get; }
    QuestionOption[] Options { get; }
    QuestionRule[] Rules { get; }
    QuestionTiming? Timing { get; }
    int RoundPageNumber { get; }
    int QuestionPageNumber { get; }

    static IQuestionGameAdminContext Create(
        Session currentSession,
        Round round,
        Question question,
        QuestionOption[] options,
        QuestionRule[] rules,
        QuestionTiming? timing,
        int roundPageNumber,
        int questionPageNumber,
        IGameAdminContext baseContext)
    {
        return new QuestionGameAdminContext(
            round,
            question,
            options,
            rules,
            timing,
            roundPageNumber,
            questionPageNumber,
            baseContext.Game,
            baseContext.Session,
            baseContext.TargetBot,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record QuestionGameAdminContext(
        Round Round,
        Question Question,
        QuestionOption[] Options,
        QuestionRule[] Rules,
        QuestionTiming? Timing,
        int RoundPageNumber,
        int QuestionPageNumber,
        Game Game,
        Session Session,
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IQuestionGameAdminContext;
}