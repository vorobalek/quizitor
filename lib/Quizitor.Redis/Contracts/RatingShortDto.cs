namespace Quizitor.Redis.Contracts;

public record RatingShortDto(
    int SessionId,
    DateTimeOffset LastUpdatedAt,
    RatingShortLineDto[] Lines) : IRating
{
    internal const string SessionRedisStageKey = "Rating:Short:{0}:Stage";
    internal const string SessionRedisFinalKey = "Rating:Short:{0}:Final";
}