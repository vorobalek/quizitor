using Microsoft.Extensions.Options;
using Prometheus;
using Quizitor.Common;
using Quizitor.Kafka;
using Quizitor.Sender.Services.Infrastructure;

namespace Quizitor.Sender.Services;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class DeleteMessageKafkaConsumerTask(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<KafkaOptions> options,
    ILogger<DeleteMessageKafkaConsumerTask> logger)
    : SlaSenderKafkaConsumerTask<long>(
        serviceScopeFactory,
        options,
        logger)
{
    private static readonly Histogram SenderDeleteMessageBackOfficeHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}sender_delete_message_backoffice",
        "Histogram of backoffice deleteMessage call.",
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

    private static readonly Histogram SenderDeleteMessageBotHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}sender_delete_message_bot",
        "Histogram of bot's deleteMessage call.",
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

    protected override string Method => "deleteMessage";
    protected override string TopicMain => KafkaTopics.DeleteMessageTopicName;
    protected override string TopicBot => KafkaTopics.DeleteMessageBotTopicName;
    protected override Histogram BackOfficeHistogram => SenderDeleteMessageBackOfficeHistogram;
    protected override Histogram BotHistogram => SenderDeleteMessageBotHistogram;
}