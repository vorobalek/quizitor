using System.Text.Json;
using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Events.Services.Kafka;

internal sealed class QuestionTimingStopKafkaProducer(
    IKafkaProducerFactory<int, string> producerFactory)
    : KafkaProducer<int, string>(
            producerFactory),
        IQuestionTimingStopKafkaProducer
{
    public Task ProduceAsync(QuestionTimingStopDto dto, CancellationToken cancellationToken)
    {
        return ProduceAsync(
            KafkaTopics.QuestionTimingStopTopicName,
            dto.TimingId,
            JsonSerializer.Serialize(dto),
            cancellationToken);
    }
}