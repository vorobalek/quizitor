using Quizitor.Common;
using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;
using Telegram.Bot.Requests;

namespace Quizitor.Bots.Services.Kafka.Producers.AnswerCallbackQuery;

internal sealed class AnswerCallbackQueryKafkaProducer(
    IKafkaProducerFactory<long, string> producerFactory)
    : SenderKafkaProducer<long, AnswerCallbackQueryRequest>(producerFactory),
        IAnswerCallbackQueryKafkaProducer
{
    protected override string Topic => KafkaTopics.AnswerCallbackQueryTopicName;
    protected override string BotTopic => KafkaTopics.AnswerCallbackQueryBotTopicName;

    protected override long GetKey(AnswerCallbackQueryRequest request, UpdateContext updateContext)
    {
        return updateContext.Update.GetUser().Id;
    }
}