namespace Quizitor.Kafka.Contracts;

public record QuestionTimingNotifyDto(
    int TimingId,
    DateTimeOffset InitiatedAt);