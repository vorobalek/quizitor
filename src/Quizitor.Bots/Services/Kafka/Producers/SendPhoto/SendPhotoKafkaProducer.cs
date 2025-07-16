using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;
using Telegram.Bot.Requests;

namespace Quizitor.Bots.Services.Kafka.Producers.SendPhoto;

internal sealed class SendPhotoKafkaProducer(IKafkaProducerFactory<long, string> producerFactory) :
    SenderKafkaProducer<long, SendPhotoRequest>(producerFactory),
    ISendPhotoKafkaProducer
{
    protected override string Topic => KafkaTopics.SendPhotoTopicName;
    protected override string BotTopic => KafkaTopics.SendPhotoBotTopicName;

    protected override long GetKey(SendPhotoRequest request, UpdateContext updateContext)
    {
        return request.ChatId.Identifier!.Value;
    }
}