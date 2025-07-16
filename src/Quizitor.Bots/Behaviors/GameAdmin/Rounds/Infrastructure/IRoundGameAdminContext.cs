using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.GameAdmin.Rounds.Infrastructure;

internal interface IRoundGameAdminContext : IGameAdminContext
{
    Round Round { get; }
    Question[] Questions { get; }
    int RoundPageNumber { get; }
    int QuestionPageNumber { get; }
    int QuestionPageCount { get; }

    static IRoundGameAdminContext Create(
        Round round,
        Question[] questions,
        int roundPageNumber,
        int questionPageNumber,
        int questionPageCount,
        IGameAdminContext baseContext)
    {
        return new RoundGameAdminContext(
            round,
            questions,
            roundPageNumber,
            questionPageNumber,
            questionPageCount,
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

    private record RoundGameAdminContext(
        Round Round,
        Question[] Questions,
        int RoundPageNumber,
        int QuestionPageNumber,
        int QuestionPageCount,
        Game Game,
        Session Session,
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IRoundGameAdminContext;
}