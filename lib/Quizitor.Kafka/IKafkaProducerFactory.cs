using Confluent.Kafka;

namespace Quizitor.Kafka;

public interface IKafkaProducerFactory<TKey, TMessage> : IDisposable
{
    IProducer<TKey, TMessage> Create(string name);
}

public interface IKafkaProducerFactory<TMessage> : IKafkaProducerFactory<Null, TMessage>;