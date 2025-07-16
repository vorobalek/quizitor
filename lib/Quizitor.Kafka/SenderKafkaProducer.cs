using System.Text.Json;
using Quizitor.Kafka.Contracts;
using Telegram.Bot;

namespace Quizitor.Kafka;

public abstract class SenderKafkaProducer<TKey, TRequest>(
    IKafkaProducerFactory<TKey, string> producerFactory) :
    KafkaProducer<TKey, string>(producerFactory),
    ISenderKafkaProducer<TRequest>
{
    protected abstract string Topic { get; }
    protected abstract string BotTopic { get; }

    public Task ProduceAsync(
        TRequest request,
        UpdateContext updateContext,
        CancellationToken cancellationToken)
    {
        var content = JsonSerializer.Serialize(request, JsonBotAPI.Options);
        var message = JsonSerializer.Serialize(
            new SenderContext(
                content,
                updateContext),
            JsonBotAPI.Options);
        return ProduceAsync(
            Topic,
            GetKey(request, updateContext),
            message,
            cancellationToken);
    }

    public Task ProduceBotAsync(
        int botId,
        TRequest request,
        UpdateContext updateContext,
        CancellationToken cancellationToken)
    {
        var topic = string.Format(BotTopic, botId);
        var content = JsonSerializer.Serialize(request, JsonBotAPI.Options);
        var message = JsonSerializer.Serialize(
            new SenderContext(
                content,
                updateContext),
            JsonBotAPI.Options);
        return ProduceAsync(
            topic,
            GetKey(request, updateContext),
            message,
            cancellationToken);
    }

    protected abstract TKey GetKey(TRequest request, UpdateContext updateContext);
}