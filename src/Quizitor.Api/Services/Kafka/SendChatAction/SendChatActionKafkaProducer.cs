using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;
using Telegram.Bot.Requests;

namespace Quizitor.Api.Services.Kafka.SendChatAction;

internal sealed class SendChatActionKafkaProducer(IKafkaProducerFactory<long, string> producerFactory) :
    SenderKafkaProducer<long, SendChatActionRequest>(producerFactory),
    ISendChatActionKafkaProducer
{
    protected override string Topic => KafkaTopics.SendChatActionTopicName;
    protected override string BotTopic => KafkaTopics.SendChatActionBotTopicName;

    protected override long GetKey(SendChatActionRequest request, UpdateContext updateContext)
    {
        return request.ChatId.Identifier!.Value;
    }
}