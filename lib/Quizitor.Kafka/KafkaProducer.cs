using Confluent.Kafka;

namespace Quizitor.Kafka;

public abstract class KafkaProducer<TKey, TMessage>(IKafkaProducerFactory<TKey, TMessage> producerFactory)
{
    protected async Task ProduceAsync(
        string topic,
        TKey key,
        TMessage message,
        CancellationToken cancellationToken)
    {
        var producer = producerFactory.Create(topic);

        await producer.ProduceAsync(
            topic,
            new Message<TKey, TMessage>
            {
                Key = key,
                Value = message
            },
            cancellationToken);
    }
}