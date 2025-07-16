using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;
using Quizitor.Redis.Contracts;

namespace Quizitor.Bots.Behaviors.GameServer.Rating.Infrastructure;

internal interface IRatingFinalShortGameServerContext : IGameServerContext
{
    RatingShortLineDto[] Lines { get; }
    int RatingPageNumber { get; }
    TimeSpan? TimeSinceLastUpdate { get; }

    static IRatingFinalShortGameServerContext Create(
        RatingShortLineDto[] lines,
        int ratingPageNumber,
        TimeSpan? timeSinceLastUpdate,
        IGameServerContext baseContext)
    {
        return new RatingFinalShortGameServerContext(
            lines,
            ratingPageNumber,
            timeSinceLastUpdate,
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

    private record RatingFinalShortGameServerContext(
        RatingShortLineDto[] Lines,
        int RatingPageNumber,
        TimeSpan? TimeSinceLastUpdate,
        ISessionTeamInfo? SessionTeamInfo,
        Game Game,
        Session Session,
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IRatingFinalShortGameServerContext;
}