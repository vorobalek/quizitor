namespace Quizitor.Kafka.Contracts;

// ReSharper disable once ClassNeverInstantiated.Global
public record SenderContext(
    string Content,
    UpdateContext UpdateContext);