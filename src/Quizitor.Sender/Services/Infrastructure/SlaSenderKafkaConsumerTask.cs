using Microsoft.Extensions.Options;
using Prometheus;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Sender.Services.Infrastructure;

internal abstract class SlaSenderKafkaConsumerTask<TKey>(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<KafkaOptions> options,
    ILogger logger) :
    DummySenderKafkaConsumerTask<TKey>(
        serviceScopeFactory,
        options,
        logger)
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Histogram E2EBackOfficeHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}e2e_backoffice",
        "Histogram of e2e backoffice timings.",
        new HistogramConfiguration
        {
            Buckets =
            [
                .05, .1, .2, .3, .4, .5, .6, .7, .8, .9, 1, 2, 5, 10, 30, 60
            ],
            LabelNames =
            [
                "is_test"
            ]
        });

    // ReSharper disable once StaticMemberInGenericType
    private static readonly Histogram E2EBotHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}e2e_bot",
        "Histogram of e2e bot's timings.",
        new HistogramConfiguration
        {
            Buckets =
            [
                .05, .1, .2, .3, .4, .5, .6, .7, .8, .9, 1, 2, 5, 10, 30, 60
            ],
            LabelNames =
            [
                "bot_id",
                "is_test"
            ]
        });

    protected override async Task PostProcessAsync(
        AsyncServiceScope asyncScope,
        SenderContext senderContext,
        CancellationToken cancellationToken)
    {
        if (senderContext.UpdateContext.InitiatedAt.HasValue)
        {
            var dbContextProvider = asyncScope.ServiceProvider.GetRequiredService<IDbContextProvider>();
            var serverTime = await dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);
            var totalSeconds = (serverTime - senderContext.UpdateContext.InitiatedAt.Value).TotalSeconds;

            if (senderContext.UpdateContext.BotId.HasValue)
                E2EBotHistogram.WithLabels([
                    senderContext.UpdateContext.BotId.Value.ToString(),
                    senderContext.UpdateContext.IsTest.ToString()
                ]).Observe(totalSeconds);
            else
                E2EBackOfficeHistogram.WithLabels([senderContext.UpdateContext.IsTest.ToString()]).Observe(totalSeconds);
        }
    }
}