using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Data.Entities.Events;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions.Infrastructure;

internal interface IQuestionStopGameAdminContext : IQuestionGameAdminContext
{
    QuestionTiming CurrentTiming { get; }
    QuestionTimingNotify? TimingNotify { get; }
    QuestionTimingStop? TimingStop { get; }
    Question? NextQuestion { get; }
    Bot[] GameBots { get; }
    User[] GameUsers { get; }
    Bot[] AdminBots { get; }
    User[] AdminUsers { get; }
    DateTimeOffset ServerTime { get; }

    static IQuestionStopGameAdminContext Create(
        QuestionTiming currentTiming,
        QuestionTimingNotify? timingNotify,
        QuestionTimingStop? timingStop,
        Question? nextQuestion,
        Bot[] gameBots,
        User[] gameUsers,
        Bot[] adminBots,
        User[] adminUsers,
        DateTimeOffset serverTime,
        IQuestionGameAdminContext baseContext)
    {
        return new QuestionStopGameAdminContext(
            currentTiming,
            timingNotify,
            timingStop,
            nextQuestion,
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

    private record QuestionStopGameAdminContext(
        QuestionTiming CurrentTiming,
        QuestionTimingNotify? TimingNotify,
        QuestionTimingStop? TimingStop,
        Question? NextQuestion,
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
        IIdentity Identity) : IQuestionStopGameAdminContext;
}