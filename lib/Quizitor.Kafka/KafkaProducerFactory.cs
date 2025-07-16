using System.Collections.Concurrent;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Quizitor.Kafka;

internal class KafkaProducerFactory<TKey, TMessage>(IOptions<KafkaOptions> options) :
    IKafkaProducerFactory<TKey, TMessage>
{
    private readonly ConcurrentDictionary<string, IProducer<TKey, TMessage>> _producers = new();

    public IProducer<TKey, TMessage> Create(string name)
    {
        if (_producers.TryGetValue(name, out var producer))
            return producer;

        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            CompressionType = options.Value.CompressionType
        };

        producer = new ProducerBuilder<TKey, TMessage>(config).Build();

        _producers.TryAdd(name, producer);
        return producer;
    }

    public void Dispose()
    {
        foreach (var producer in _producers.Values)
        {
            producer.Flush();
            producer.Dispose();
        }
    }
}

internal sealed class KafkaProducerFactory<TMessage>(IOptions<KafkaOptions> options) :
    KafkaProducerFactory<Null, TMessage>(options),
    IKafkaProducerFactory<TMessage>;