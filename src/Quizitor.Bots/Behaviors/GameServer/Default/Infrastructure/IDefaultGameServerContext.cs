using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.GameServer.Default.Infrastructure;

internal interface IAnswer
{
    Round Round { get; }
    Question Question { get; }
    QuestionTiming QuestionTiming { get; }
    QuestionOption[] Options { get; }
    QuestionOption? Choice { get; }
    QuestionRule[] Rules { get; }
    bool HasReachedAttemptsCount { get; }
    int AttemptsCountRemaining { get; }
    Bot[] GameBots { get; }
    Bot[] AdminBots { get; }
    User[] AdminUsers { get; }
    DateTimeOffset InitiatedAt { get; }

    static IAnswer Create(
        Round round,
        Question question,
        QuestionTiming questionTiming,
        QuestionOption[] options,
        QuestionOption? choice,
        QuestionRule[] rules,
        bool hasReachedAttemptsCount,
        int attemptsCountRemaining,
        Bot[] gameBots,
        Bot[] adminBots,
        User[] adminUsers,
        DateTimeOffset initiatedAt)
    {
        return new Answer(
            round,
            question,
            questionTiming,
            options,
            choice,
            rules,
            hasReachedAttemptsCount,
            attemptsCountRemaining,
            gameBots,
            adminBots,
            adminUsers,
            initiatedAt);
    }

    private record Answer(
        Round Round,
        Question Question,
        QuestionTiming QuestionTiming,
        QuestionOption[] Options,
        QuestionOption? Choice,
        QuestionRule[] Rules,
        bool HasReachedAttemptsCount,
        int AttemptsCountRemaining,
        Bot[] GameBots,
        Bot[] AdminBots,
        User[] AdminUsers,
        DateTimeOffset InitiatedAt) : IAnswer;
}

internal interface IDefaultGameServerContext : IGameServerContext
{
    IAnswer? Answer { get; }

    static IDefaultGameServerContext Create(
        IAnswer? answer,
        IGameServerContext baseContext)
    {
        return new DefaultGameServerContext(
            answer,
            baseContext.SessionTeamInfo,
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

    private record DefaultGameServerContext(
        IAnswer? Answer,
        ISessionTeamInfo? SessionTeamInfo,
        Game Game,
        Session Session,
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IDefaultGameServerContext;
}