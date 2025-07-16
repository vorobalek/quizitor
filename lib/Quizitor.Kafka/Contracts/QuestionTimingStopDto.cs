namespace Quizitor.Kafka.Contracts;

public record QuestionTimingStopDto(
    int TimingId,
    int RoundPageNumber,
    int QuestionPageNumber,
    DateTimeOffset InitiatedAt);