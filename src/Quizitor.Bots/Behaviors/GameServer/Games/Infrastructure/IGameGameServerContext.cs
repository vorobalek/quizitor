using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.GameServer.Games.Infrastructure;

internal interface IGameGameServerContext : IGameServerContext
{
    int RoundsCount { get; }
    int QuestionsCount { get; }
    int ConnectedUsersCount { get; }

    static IGameGameServerContext Create(
        int roundsCount,
        int questionsCount,
        int connectedUsersCount,
        IGameServerContext baseContext)
    {
        return new GameGameServerContext(
            roundsCount,
            questionsCount,
            connectedUsersCount,
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

    private record GameGameServerContext(
        int RoundsCount,
        int QuestionsCount,
        int ConnectedUsersCount,
        ISessionTeamInfo? SessionTeamInfo,
        Game Game,
        Session Session,
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IGameGameServerContext;
}