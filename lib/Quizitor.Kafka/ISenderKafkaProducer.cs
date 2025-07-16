using Quizitor.Kafka.Contracts;

namespace Quizitor.Kafka;

public interface ISenderKafkaProducer<in TRequest>
{
    Task ProduceAsync(
        TRequest request,
        UpdateContext updateContext,
        CancellationToken cancellationToken);

    Task ProduceBotAsync(
        int botId,
        TRequest request,
        UpdateContext updateContext,
        CancellationToken cancellationToken);
}