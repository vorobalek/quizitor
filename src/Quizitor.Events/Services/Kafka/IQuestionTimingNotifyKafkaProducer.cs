using Quizitor.Kafka.Contracts;

namespace Quizitor.Events.Services.Kafka;

internal interface IQuestionTimingNotifyKafkaProducer
{
    Task ProduceAsync(QuestionTimingNotifyDto dto, CancellationToken cancellationToken);
}