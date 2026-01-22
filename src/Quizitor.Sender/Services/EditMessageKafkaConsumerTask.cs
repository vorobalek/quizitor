using Microsoft.Extensions.Options;
using Prometheus;
using Quizitor.Common;
using Quizitor.Kafka;
using Quizitor.Sender.Services.Infrastructure;

namespace Quizitor.Sender.Services;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class EditMessageKafkaConsumerTask(
    IServiceScopeFactory serviceScopeFactory,
    IBotListCache botListCache,
    IOptions<KafkaOptions> options,
    ILogger<EditMessageKafkaConsumerTask> logger)
    : SlaSenderKafkaConsumerTask<long>(
        serviceScopeFactory,
        botListCache,
        options,
        logger)
{
    private static readonly Histogram SenderEditMessageBackOfficeHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}sender_edit_message_backoffice",
        "Histogram of backoffice editMessage call.",
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

    private static readonly Histogram SenderEditMessageBotHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}sender_edit_message_bot",
        "Histogram of bot's editMessage call.",
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

    protected override string Method => "editMessageText";
    protected override string TopicMain => KafkaTopics.EditMessageTopicName;
    protected override string TopicBot => KafkaTopics.EditMessageBotTopicName;
    protected override Histogram BackOfficeHistogram => SenderEditMessageBackOfficeHistogram;
    protected override Histogram BotHistogram => SenderEditMessageBotHistogram;
}