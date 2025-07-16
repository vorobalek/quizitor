using System.Text.Json;
using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Events.Services.Kafka;

internal sealed class QuestionTimingNotifyKafkaProducer(
    IKafkaProducerFactory<int, string> producerFactory)
    : KafkaProducer<int, string>(
            producerFactory),
        IQuestionTimingNotifyKafkaProducer
{
    public Task ProduceAsync(QuestionTimingNotifyDto dto, CancellationToken cancellationToken)
    {
        return ProduceAsync(
            KafkaTopics.QuestionTimingNotifyTopicName,
            dto.TimingId,
            JsonSerializer.Serialize(dto),
            cancellationToken);
    }
}