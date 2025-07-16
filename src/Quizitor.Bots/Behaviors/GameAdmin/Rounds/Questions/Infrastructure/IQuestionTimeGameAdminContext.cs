using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions.Infrastructure;

internal interface IQuestionTimeGameAdminContext : IGameAdminContext
{
    Round Round { get; }
    Question Question { get; }
    QuestionTiming QuestionTiming { get; }
    DateTimeOffset ServerTime { get; }

    static IQuestionTimeGameAdminContext Create(
        Round round,
        Question question,
        QuestionTiming timing,
        DateTimeOffset serverTime,
        IGameAdminContext baseContext)
    {
        return new QuestionTimeGameAdminContext(
            round,
            question,
            timing,
            serverTime,
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

    private record QuestionTimeGameAdminContext(
        Round Round,
        Question Question,
        QuestionTiming QuestionTiming,
        DateTimeOffset ServerTime,
        Game Game,
        Session Session,
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IQuestionTimeGameAdminContext;
}