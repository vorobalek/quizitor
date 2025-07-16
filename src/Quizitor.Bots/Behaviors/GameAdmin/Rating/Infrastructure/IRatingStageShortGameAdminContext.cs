using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;
using Quizitor.Redis.Contracts;

namespace Quizitor.Bots.Behaviors.GameAdmin.Rating.Infrastructure;

internal interface IRatingStageShortGameAdminContext : IGameAdminContext
{
    RatingShortLineDto[] Lines { get; }
    int RatingPageNumber { get; }
    TimeSpan? TimeSinceLastUpdate { get; }

    static IRatingStageShortGameAdminContext Create(
        RatingShortLineDto[] lines,
        int ratingPageNumber,
        TimeSpan? timeSinceLastUpdate,
        IGameAdminContext baseContext)
    {
        return new RatingStageShortGameAdminContext(
            lines,
            ratingPageNumber,
            timeSinceLastUpdate,
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

    private record RatingStageShortGameAdminContext(
        RatingShortLineDto[] Lines,
        int RatingPageNumber,
        TimeSpan? TimeSinceLastUpdate,
        Game Game,
        Session Session,
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IRatingStageShortGameAdminContext;
}