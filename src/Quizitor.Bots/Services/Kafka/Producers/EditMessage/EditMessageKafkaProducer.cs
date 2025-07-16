using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;
using Telegram.Bot.Requests;

namespace Quizitor.Bots.Services.Kafka.Producers.EditMessage;

internal sealed class EditMessageKafkaProducer(
    IKafkaProducerFactory<long, string> producerFactory)
    : SenderKafkaProducer<long, EditMessageTextRequest>(
        producerFactory), IEditMessageKafkaProducer
{
    protected override string Topic => KafkaTopics.EditMessageTopicName;
    protected override string BotTopic => KafkaTopics.EditMessageBotTopicName;

    protected override long GetKey(EditMessageTextRequest request, UpdateContext updateContext)
    {
        return request.ChatId.Identifier!.Value;
    }
}