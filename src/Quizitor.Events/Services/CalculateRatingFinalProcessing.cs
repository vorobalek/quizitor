using Prometheus;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Redis.Contracts;
using Quizitor.Redis.Storage.Rating;

namespace Quizitor.Events.Services;

internal sealed partial class CalculateRatingFinalProcessing(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<CalculateRatingFinalProcessing> logger) : BackgroundService
{
    private static readonly Histogram CalculateRatingFinalHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}events_calculate_rating_final",
        "Histogram of final rating calculation.",
        new HistogramConfiguration
        {
            Buckets =
            [
                .05, .1, .2, .3, .4, .5, .6, .7, .8, .9, 1, 2, 5, 10, 30, 60
            ]
        });

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteOnceAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                LogAnExceptionOccurredWhileCalculatingFinalRating(logger, exception);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
    {
        var timer = CalculateRatingFinalHistogram.NewTimer();
        await using var asyncScope = serviceScopeFactory.CreateAsyncScope();
        var services = asyncScope.ServiceProvider;
        var shouldCollect = false;

        var dbContext = services.GetRequiredService<IDbContextProvider>();

        var sessions = await dbContext
            .Sessions
            .GetAllNoTrackingAsync(cancellationToken);

        foreach (var session in sessions)
        {
            if (!session.SyncRating) continue;

            var ratingShortStageRedisStorage = services.GetRequiredService<IRatingShortStageRedisStorage>();
            var ratingFullStageRedisStorage = services.GetRequiredService<IRatingFullStageRedisStorage>();
            var ratingShortFinalRedisStorage = services.GetRequiredService<IRatingShortFinalRedisStorage>();
            var ratingFullFinalRedisStorage = services.GetRequiredService<IRatingFullFinalRedisStorage>();

            var stageRatingShort = await ratingShortStageRedisStorage
                .ReadAsync(
                    session.Id,
                    cancellationToken);
            var stageRatingFull = await ratingFullStageRedisStorage
                .ReadAsync(
                    session.Id,
                    cancellationToken);

            var oldRatingShort = await ratingShortFinalRedisStorage
                .ReadAsync(
                    session.Id,
                    cancellationToken);
            var oldRatingFull = await ratingFullFinalRedisStorage
                .ReadAsync(
                    session.Id,
                    cancellationToken);

            var serverDateTime = await dbContext.GetServerDateTimeOffsetAsync(cancellationToken);

            var newRatingShort =
                new RatingShortDto(
                    session.Id,
                    serverDateTime,
                    stageRatingShort?.Lines ?? []);
            var newRatingFull =
                new RatingFullDto(
                    session.Id,
                    serverDateTime,
                    stageRatingFull?.Lines ?? []);
            if (!(oldRatingShort?.Lines ?? []).SequenceEqual(newRatingShort.Lines))
            {
                await ratingShortFinalRedisStorage
                    .UpsertAsync(
                        newRatingShort,
                        cancellationToken);
                shouldCollect = true;
            }

            if (!(oldRatingFull?.Lines ?? []).SequenceEqual(newRatingFull.Lines))
            {
                await ratingFullFinalRedisStorage
                    .UpsertAsync(
                        newRatingFull,
                        cancellationToken);
                shouldCollect = true;
            }
        }

        if (shouldCollect)
        {
            timer.ObserveDuration();
        }
    }

    [LoggerMessage(LogLevel.Error, "An exception occurred while calculating final rating")]
    static partial void LogAnExceptionOccurredWhileCalculatingFinalRating(ILogger<CalculateRatingFinalProcessing> logger, Exception exception);
}