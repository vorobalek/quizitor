using Prometheus;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Redis.Contracts;
using Quizitor.Redis.Storage.Rating;

namespace Quizitor.Events.Services;

internal sealed partial class CalculateRatingStageProcessing(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<CalculateRatingStageProcessing> logger) : BackgroundService
{
    private static readonly Histogram CalculateRatingStageHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}events_calculate_rating_stage",
        "Histogram of stage rating calculation.",
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
                LogAnExceptionOccurredWhileCalculatingStageRating(logger, exception);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
    {
        var timer = CalculateRatingStageHistogram.NewTimer();
        await using var asyncScope = serviceScopeFactory.CreateAsyncScope();
        var shouldCollect = false;

        var dbContext = asyncScope.ServiceProvider.GetRequiredService<IDbContextProvider>();

        var sessions = await dbContext
            .Sessions
            .GetAllNoTrackingAsync(cancellationToken);

        foreach (var session in sessions)
        {
            var ratingShortStorage = asyncScope.ServiceProvider.GetRequiredService<IRatingShortStageRedisStorage>();
            var ratingFullStorage = asyncScope.ServiceProvider.GetRequiredService<IRatingFullStageRedisStorage>();

            var oldRatingShort = await ratingShortStorage
                .ReadAsync(
                    session.Id,
                    cancellationToken);
            var oldRatingFull = await ratingFullStorage
                .ReadAsync(
                    session.Id,
                    cancellationToken);

            var ratingShortLines = new List<RatingShortLineDto>();
            var ratingFullLines = new List<RatingFullLineDto>();

            var teamsSubmissions = await dbContext
                .Submissions
                .GetForRatingCalculationGroupByTeamNotNullNoTrackingAsync(
                    session.Id,
                    cancellationToken);

            foreach (var teamSubmissions in teamsSubmissions)
            {
                if (teamSubmissions.Key is null) continue;
                CalculateSubmissions(
                    null,
                    null,
                    teamSubmissions.Key.Id,
                    teamSubmissions.Key.Name,
                    teamSubmissions,
                    out var teamRatingShortLine,
                    out var teamRatingFullLine);
                ratingShortLines.Add(teamRatingShortLine);
                ratingFullLines.Add(teamRatingFullLine);
            }

            var usersSubmissions = await dbContext
                .Submissions
                .GetForRatingCalculationGroupByUserWithoutTeamNoTrackingAsync(
                    session.Id,
                    cancellationToken);

            foreach (var userSubmissions in usersSubmissions)
            {
                CalculateSubmissions(
                    userSubmissions.Key.Id,
                    userSubmissions.Key.GetFullName(),
                    null,
                    null,
                    userSubmissions,
                    out var userRatingShortLine,
                    out var userRatingFullLine);
                ratingShortLines.Add(userRatingShortLine);
                ratingFullLines.Add(userRatingFullLine);
            }

            var serverDateTime = await dbContext.GetServerDateTimeOffsetAsync(cancellationToken);

            var newRatingShort =
                new RatingShortDto(
                    session.Id,
                    serverDateTime,
                    [.. ratingShortLines]);
            var newRatingFull =
                new RatingFullDto(
                    session.Id,
                    serverDateTime,
                    [.. ratingFullLines]);
            if (!(oldRatingShort?.Lines ?? []).SequenceEqual(newRatingShort.Lines))
            {
                await ratingShortStorage
                    .UpsertAsync(
                        newRatingShort,
                        cancellationToken);
                shouldCollect = true;
            }

            if (!(oldRatingFull?.Lines ?? []).SequenceEqual(newRatingFull.Lines))
            {
                await ratingFullStorage
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

    private static void CalculateSubmissions(
        long? userId,
        string? userFullName,
        int? teamId,
        string? teamFullName,
        IEnumerable<Submission> submissions,
        out RatingShortLineDto ratingShortLine,
        out RatingFullLineDto ratingFullLine)
    {
        ratingFullLine = new RatingFullLineDto(
            userId,
            userFullName,
            teamId,
            teamFullName,
            [],
            []);

        var totalScore = 0;
        var totalTime = 0;
        foreach (var submission in submissions
                     .OrderBy(x => x.Question.Round.Number)
                     .ThenBy(x => x.Question.Number))
        {
            if (!ratingFullLine.ScorePerRound.TryGetValue(
                    submission.Question.Round.Number,
                    out var scorePerQuestion))
            {
                scorePerQuestion = [];
                ratingFullLine.ScorePerRound[submission.Question.Round.Number] = scorePerQuestion;
            }

            scorePerQuestion[submission.Question.Number] = submission.Score;
            totalScore += submission.Score;

            var submissionTime = submission.Score > 0
                ? submission.Time
                : 0;

            if (!ratingFullLine.TimePerRound.TryGetValue(submission.Question.Round.Number, out var timePerQuestion))
            {
                timePerQuestion = [];
                ratingFullLine.TimePerRound[submission.Question.Round.Number] = timePerQuestion;
            }

            timePerQuestion[submission.Question.Number] = submissionTime;
            totalTime += submissionTime;
        }

        ratingShortLine = new RatingShortLineDto(
            userId,
            userFullName,
            teamId,
            teamFullName,
            totalScore,
            totalTime);
    }

    [LoggerMessage(LogLevel.Error, "An exception occurred while calculating stage rating")]
    static partial void LogAnExceptionOccurredWhileCalculatingStageRating(ILogger<CalculateRatingStageProcessing> logger, Exception exception);
}