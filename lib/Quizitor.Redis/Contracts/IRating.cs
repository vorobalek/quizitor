namespace Quizitor.Redis.Contracts;

public interface IRating
{
    int SessionId { get; }
    DateTimeOffset LastUpdatedAt { get; }
}