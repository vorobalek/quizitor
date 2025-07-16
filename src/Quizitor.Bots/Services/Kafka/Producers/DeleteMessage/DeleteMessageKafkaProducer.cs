using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;
using Telegram.Bot.Requests;

namespace Quizitor.Bots.Services.Kafka.Producers.DeleteMessage;

internal sealed class DeleteMessageKafkaProducer(
    IKafkaProducerFactory<long, string> producerFactory)
    : SenderKafkaProducer<long, DeleteMessageRequest>(
        producerFactory), IDeleteMessageKafkaProducer
{
    protected override string Topic => KafkaTopics.DeleteMessageTopicName;
    protected override string BotTopic => KafkaTopics.DeleteMessageBotTopicName;

    protected override long GetKey(DeleteMessageRequest request, UpdateContext updateContext)
    {
        return request.ChatId.Identifier!.Value;
    }
}