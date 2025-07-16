using Quizitor.Common;
using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;
using Telegram.Bot;
using Telegram.Bot.Types;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Quizitor.Api.Services.Kafka.Updates;

internal sealed class UpdateKafkaProducer(IKafkaProducerFactory<long, string> producerFactory) :
    KafkaProducer<long, string>(producerFactory),
    IUpdateKafkaProducer
{
    public async Task ProduceAsync(
        Update update,
        DateTimeOffset? initiatedAt,
        bool isTest,
        CancellationToken cancellationToken)
    {
        var message = JsonSerializer.Serialize(
            new UpdateContext(
                null,
                update,
                initiatedAt,
                isTest),
            JsonBotAPI.Options);
        await ProduceAsync(
            KafkaTopics.UpdateTopicName,
            update.GetUser().Id,
            message,
            cancellationToken);
    }

    public async Task ProduceBotAsync(
        int botId,
        Update update,
        DateTimeOffset? initiatedAt,
        bool isTest,
        CancellationToken cancellationToken)
    {
        var topic = string.Format(KafkaTopics.UpdateBotTopicName, botId);
        var message = JsonSerializer.Serialize(
            new UpdateContext(
                botId,
                update,
                initiatedAt,
                isTest),
            JsonBotAPI.Options);
        await ProduceAsync(
            topic,
            update.GetUser().Id,
            message,
            cancellationToken);
    }
}