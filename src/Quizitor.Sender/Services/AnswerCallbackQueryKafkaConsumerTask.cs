using Microsoft.Extensions.Options;
using Prometheus;
using Quizitor.Common;
using Quizitor.Kafka;
using Quizitor.Sender.Services.Infrastructure;

namespace Quizitor.Sender.Services;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class AnswerCallbackQueryKafkaConsumerTask(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<KafkaOptions> options,
    ILogger<AnswerCallbackQueryKafkaConsumerTask> logger)
    : SlaSenderKafkaConsumerTask<long>(
        serviceScopeFactory,
        options,
        logger)
{
    private static readonly Histogram SenderSendAnswerCallbackQueryBackOfficeHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}sender_answer_callback_query_backoffice",
        "Histogram of backoffice answerCallbackQuery call.",
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

    private static readonly Histogram SenderSendAnswerCallbackQueryBotHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}sender_answer_callback_query_bot",
        "Histogram of bot's answerCallbackQuery call.",
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

    protected override string Method => "answerCallbackQuery";
    protected override string TopicMain => KafkaTopics.AnswerCallbackQueryTopicName;
    protected override string TopicBot => KafkaTopics.AnswerCallbackQueryBotTopicName;
    protected override Histogram BackOfficeHistogram => SenderSendAnswerCallbackQueryBackOfficeHistogram;
    protected override Histogram BotHistogram => SenderSendAnswerCallbackQueryBotHistogram;
}