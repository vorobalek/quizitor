using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.GameAdmin.Games.Infrastructure;

internal interface IGameGameAdminContext : IGameAdminContext
{
    int RoundsCount { get; }
    int QuestionsCount { get; }
    int SubmissionsCount { get; }
    int ConnectedUsersCount { get; }

    static IGameGameAdminContext Create(
        int roundsCount,
        int questionsCount,
        int submissionsCount,
        int connectedUsersCount,
        IGameAdminContext baseContext)
    {
        return new GameGameAdminContext(
            roundsCount,
            questionsCount,
            submissionsCount,
            connectedUsersCount,
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

    private record GameGameAdminContext(
        int RoundsCount,
        int QuestionsCount,
        int SubmissionsCount,
        int ConnectedUsersCount,
        Game Game,
        Session Session,
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IGameGameAdminContext;
}