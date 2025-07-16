using System.Text.Json;
using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;
using Telegram.Bot;
using Telegram.Bot.Requests;

namespace Quizitor.Bots.Services.Kafka.Producers.SendMessage;

internal sealed class SendMessageKafkaProducer(IKafkaProducerFactory<long, string> producerFactory) :
    SenderKafkaProducer<long, SendMessageRequest>(producerFactory),
    ISendMessageKafkaProducer
{
    protected override string Topic => KafkaTopics.SendMessageTopicName;
    protected override string BotTopic => KafkaTopics.SendMessageBotTopicName;

    public async Task ProduceBatchAsync(
        SendMessageRequest request,
        UpdateContext updateContext,
        CancellationToken cancellationToken,
        params int?[] botIds)
    {
        var key = request.ChatId.Identifier!.Value;
        var content = JsonSerializer.Serialize(request, JsonBotAPI.Options);
        var message = JsonSerializer.Serialize(
            new SenderContext(
                content,
                updateContext),
            JsonBotAPI.Options);
        var topics = botIds.Distinct().Select(x =>
            x.HasValue
                ? string.Format(KafkaTopics.SendMessageBotTopicName, x)
                : KafkaTopics.SendMessageTopicName);
        await Task.WhenAll(topics
            .Select(topic =>
                ProduceAsync(
                    topic,
                    key,
                    message,
                    cancellationToken)));
    }

    protected override long GetKey(SendMessageRequest request, UpdateContext updateContext)
    {
        return request.ChatId.Identifier!.Value;
    }
}