using Prometheus;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities.Events;
using Quizitor.Data.Extensions;
using Quizitor.Data.Repositories;
using Quizitor.Events.Services.Kafka;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Events.Services;

internal sealed partial class QuestionTimingStopEventProcessing(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<QuestionTimingStopEventProcessing> logger) : BackgroundService
{
    private static readonly Histogram QuestionTimingStopHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}question_timing_stop",
        "Histogram of question timing stops.",
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
                LogAnExceptionOccurredWhileExecutingTheQuestionStopQueue(logger, exception);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
    {
        var timer = QuestionTimingStopHistogram.NewTimer();
        await using var asyncScope = serviceScopeFactory.CreateAsyncScope();
        var shouldCollect = false;

        var dbContextProvider = asyncScope.ServiceProvider.GetRequiredService<IDbContextProvider>();
        var timingStopKafkaProducer = asyncScope.ServiceProvider.GetRequiredService<IQuestionTimingStopKafkaProducer>();
        while (!cancellationToken.IsCancellationRequested)
        {
            var serverDateTime = await dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);

            var candidateEvent = await GetCandidateEventAsync(
                dbContextProvider.QuestionStopTimings,
                serverDateTime,
                cancellationToken);

            if (candidateEvent is null) break;

            var marked = await MarkCandidateItemAsync(
                dbContextProvider,
                serverDateTime,
                candidateEvent,
                cancellationToken);

            if (!marked) break;

            shouldCollect = true;
            await timingStopKafkaProducer
                .ProduceAsync(
                    new QuestionTimingStopDto(
                        candidateEvent.TimingId,
                        candidateEvent.RoundPageNumber,
                        candidateEvent.QuestionPageNumber,
                        serverDateTime),
                    cancellationToken);

            await DeleteCandidateAsync(
                dbContextProvider,
                candidateEvent,
                cancellationToken);
        }

        if (shouldCollect)
        {
            timer.ObserveDuration();
        }
    }

    private static async Task<QuestionTimingStop?> GetCandidateEventAsync(
        IQuestionStopTimingRepository questionStopTimingRepository,
        DateTimeOffset serverDateTime,
        CancellationToken cancellationToken)
    {
        var candidateItem = await questionStopTimingRepository
            .GetCandidateForProcessingAsync(
                serverDateTime,
                5,
                cancellationToken);
        return candidateItem;
    }

    private static async Task<bool> MarkCandidateItemAsync(
        IDbContextProvider dbContextProvider,
        DateTimeOffset serverDateTime,
        QuestionTimingStop candidateEvent,
        CancellationToken cancellationToken)
    {
        try
        {
            await dbContextProvider.ExecuteInTransactionAsync(
                async () =>
                {
                    candidateEvent.LastRunAt = serverDateTime;
                    await dbContextProvider
                        .QuestionStopTimings
                        .UpdateAsync(
                            candidateEvent,
                            cancellationToken);
                },
                cancellationToken);

            return true;
        }
        catch (Exception exception) when (exception.IsDbUpdateConcurrencyException)
        {
            return false;
        }
    }

    private static async Task DeleteCandidateAsync(
        IDbContextProvider dbContextProvider,
        QuestionTimingStop candidateEvent,
        CancellationToken cancellationToken)
    {
        try
        {
            await dbContextProvider.ExecuteInTransactionAsync(
                async () =>
                {
                    await dbContextProvider
                        .QuestionStopTimings
                        .RemoveAsync(
                            candidateEvent,
                            cancellationToken);
                },
                cancellationToken);
        }
        catch
        {
            // ignored
        }
    }

    [LoggerMessage(LogLevel.Error, "An exception occurred while executing the question stop queue")]
    static partial void LogAnExceptionOccurredWhileExecutingTheQuestionStopQueue(ILogger<QuestionTimingStopEventProcessing> logger, Exception exception);
}