using Microsoft.Extensions.Options;
using Prometheus;
using Quizitor.Common;
using Quizitor.Kafka;
using Quizitor.Sender.Services.Infrastructure;

namespace Quizitor.Sender.Services;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class SendMessageKafkaConsumerTask(
    IServiceScopeFactory serviceScopeFactory,
    IBotListCache botListCache,
    IOptions<KafkaOptions> options,
    ILogger<SendMessageKafkaConsumerTask> logger)
    : SlaSenderKafkaConsumerTask<long>(
        serviceScopeFactory,
        botListCache,
        options,
        logger)
{
    private static readonly Histogram SenderSendMessageBackOfficeHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}sender_send_message_backoffice",
        "Histogram of backoffice sendMessage call.",
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

    private static readonly Histogram SenderSendMessageBotHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}sender_send_message_bot",
        "Histogram of bot's sendMessage call.",
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

    protected override string Method => "sendMessage";
    protected override string TopicMain => KafkaTopics.SendMessageTopicName;
    protected override string TopicBot => KafkaTopics.SendMessageBotTopicName;
    protected override Histogram BackOfficeHistogram => SenderSendMessageBackOfficeHistogram;
    protected override Histogram BotHistogram => SenderSendMessageBotHistogram;
}