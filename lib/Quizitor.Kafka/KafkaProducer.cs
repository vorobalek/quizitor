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

public abstract class KafkaProducer<TMessage>(IKafkaProducerFactory<Null, TMessage> producerFactory)
    : KafkaProducer<Null, TMessage>(
        producerFactory)
{
    private readonly IKafkaProducerFactory<Null, TMessage> _producerFactory = producerFactory;

    protected async Task ProduceAsync(string topic, TMessage message, CancellationToken cancellationToken)
    {
        var producer = _producerFactory.Create(topic);

        await producer.ProduceAsync(
            topic,
            new Message<Null, TMessage>
            {
                Value = message
            },
            cancellationToken);
    }
}