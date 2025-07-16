using Quizitor.Kafka.Contracts;

namespace Quizitor.Events.Services.Kafka;

internal interface IQuestionTimingStopKafkaProducer
{
    Task ProduceAsync(QuestionTimingStopDto dto, CancellationToken cancellationToken);
}