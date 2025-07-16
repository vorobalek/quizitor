using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;
using Telegram.Bot.Requests;

namespace Quizitor.Bots.Services.Kafka.Producers.SendMessage;

internal interface ISendMessageKafkaProducer : ISenderKafkaProducer<SendMessageRequest>
{
    Task ProduceBatchAsync(
        SendMessageRequest request,
        UpdateContext updateContext,
        CancellationToken cancellationToken,
        params int?[] botIds);
}