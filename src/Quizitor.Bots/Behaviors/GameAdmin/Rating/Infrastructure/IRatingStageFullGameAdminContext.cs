using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;
using Quizitor.Redis.Contracts;

namespace Quizitor.Bots.Behaviors.GameAdmin.Rating.Infrastructure;

internal interface IRatingStageFullGameAdminContext : IGameAdminContext
{
    RatingFullLineDto[] Lines { get; }
    int RatingPageNumber { get; }
    TimeSpan? TimeSinceLastUpdate { get; }

    static IRatingStageFullGameAdminContext Create(
        RatingFullLineDto[] lines,
        int ratingPageNumber,
        TimeSpan? timeSinceLastUpdate,
        IGameAdminContext baseContext)
    {
        return new RatingStageFullGameAdminContext(
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

    private record RatingStageFullGameAdminContext(
        RatingFullLineDto[] Lines,
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
        IIdentity Identity) : IRatingStageFullGameAdminContext;
}