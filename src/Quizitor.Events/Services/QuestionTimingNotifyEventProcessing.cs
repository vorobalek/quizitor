using Prometheus;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities.Events;
using Quizitor.Data.Extensions;
using Quizitor.Data.Repositories;
using Quizitor.Events.Services.Kafka;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Events.Services;

internal sealed partial class QuestionTimingNotifyEventProcessing(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<QuestionTimingNotifyEventProcessing> logger) : BackgroundService
{
    private static readonly Histogram QuestionTimingNotifyHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}question_timing_notify",
        "Histogram of question timing notifications.",
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
                LogAnExceptionOccurredWhileExecutingTheQuestionNotifyQueue(logger, exception);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
    {
        var timer = QuestionTimingNotifyHistogram.NewTimer();
        await using var asyncScope = serviceScopeFactory.CreateAsyncScope();
        var services = asyncScope.ServiceProvider;
        var shouldCollect = false;

        var dbContextProvider = services.GetRequiredService<IDbContextProvider>();
        var timingNotifyKafkaProducer = services.GetRequiredService<IQuestionTimingNotifyKafkaProducer>();
        while (!cancellationToken.IsCancellationRequested)
        {
            var serverDateTime = await dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);

            var candidateEvent = await GetCandidateEventAsync(
                dbContextProvider.QuestionNotifyTimings,
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
            await timingNotifyKafkaProducer
                .ProduceAsync(
                    new QuestionTimingNotifyDto(
                        candidateEvent.TimingId,
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

    private static async Task<QuestionTimingNotify?> GetCandidateEventAsync(
        IQuestionNotifyTimingRepository questionNotifyTimingRepository,
        DateTimeOffset serverDateTime,
        CancellationToken cancellationToken)
    {
        var candidateItem = await questionNotifyTimingRepository
            .GetCandidateForProcessingAsync(
                serverDateTime,
                5,
                cancellationToken);
        return candidateItem;
    }

    private static async Task<bool> MarkCandidateItemAsync(IDbContextProvider dbContextProvider,
        DateTimeOffset serverDateTime,
        QuestionTimingNotify candidateEvent,
        CancellationToken cancellationToken)
    {
        try
        {
            await dbContextProvider.ExecuteInTransactionAsync(
                async () =>
                {
                    candidateEvent.LastRunAt = serverDateTime;
                    await dbContextProvider
                        .QuestionNotifyTimings
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
        QuestionTimingNotify candidateEvent,
        CancellationToken cancellationToken)
    {
        try
        {
            await dbContextProvider.ExecuteInTransactionAsync(
                async () =>
                {
                    await dbContextProvider
                        .QuestionNotifyTimings
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

    [LoggerMessage(LogLevel.Error, "An exception occurred while executing the question notify queue")]
    static partial void LogAnExceptionOccurredWhileExecutingTheQuestionNotifyQueue(ILogger<QuestionTimingNotifyEventProcessing> logger, Exception exception);
}