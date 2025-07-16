namespace Quizitor.Redis.Contracts;

public record RatingFullDto(
    int SessionId,
    DateTimeOffset LastUpdatedAt,
    RatingFullLineDto[] Lines) : IRating
{
    internal const string SessionRedisStageKey = "Rating:Full:{0}:Stage";
    internal const string SessionRedisFinalKey = "Rating:Full:{0}:Final";
}