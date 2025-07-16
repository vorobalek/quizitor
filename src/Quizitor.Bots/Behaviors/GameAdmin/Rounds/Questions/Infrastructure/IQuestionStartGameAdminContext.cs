using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions.Infrastructure;

internal interface IQuestionStartGameAdminContext : IQuestionGameAdminContext
{
    Round? ActiveRound { get; }
    Question? ActiveQuestion { get; }
    Bot[] GameBots { get; }
    User[] GameUsers { get; }
    Bot[] AdminBots { get; }
    User[] AdminUsers { get; }
    DateTimeOffset ServerTime { get; }

    static IQuestionStartGameAdminContext Create(
        Round? activeRound,
        Question? activeQuestion,
        Bot[] gameBots,
        User[] gameUsers,
        Bot[] adminBots,
        User[] adminUsers,
        DateTimeOffset serverTime,
        IQuestionGameAdminContext baseContext)
    {
        return new QuestionStartGameAdminContext(
            activeRound,
            activeQuestion,
            gameBots,
            gameUsers,
            adminBots,
            adminUsers,
            serverTime,
            baseContext.Round,
            baseContext.Question,
            baseContext.Options,
            baseContext.Rules,
            baseContext.Timing,
            baseContext.RoundPageNumber,
            baseContext.QuestionPageNumber,
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

    private record QuestionStartGameAdminContext(
        Round? ActiveRound,
        Question? ActiveQuestion,
        Bot[] GameBots,
        User[] GameUsers,
        Bot[] AdminBots,
        User[] AdminUsers,
        DateTimeOffset ServerTime,
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
        IIdentity Identity) : IQuestionStartGameAdminContext;
}