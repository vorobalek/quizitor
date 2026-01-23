using Microsoft.Extensions.Options;
using Prometheus;
using Quizitor.Common;
using Quizitor.Kafka;
using Quizitor.Sender.Services.Infrastructure;

namespace Quizitor.Sender.Services;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class SendChatActionKafkaConsumerTask(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<KafkaOptions> options,
    ILogger<SendMessageKafkaConsumerTask> logger)
    : DummySenderKafkaConsumerTask<long>(
        serviceScopeFactory,
        options,
        logger)
{
    private static readonly Histogram SenderSendChatActionBackOfficeHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}sender_send_chat_action_backoffice",
        "Histogram of backoffice sendChatAction call.",
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

    private static readonly Histogram SenderSendChatActionBotHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}sender_send_chat_action_bot",
        "Histogram of bot's sendChatAction call.",
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

    protected override string Method => "sendChatAction";
    protected override string TopicMain => KafkaTopics.SendChatActionTopicName;
    protected override string TopicBot => KafkaTopics.SendChatActionBotTopicName;
    protected override Histogram BackOfficeHistogram => SenderSendChatActionBackOfficeHistogram;
    protected override Histogram BotHistogram => SenderSendChatActionBotHistogram;
}