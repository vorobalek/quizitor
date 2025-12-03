using Quizitor.Common;

namespace Quizitor.Redis.Configuration;

public static class RedisConfiguration
{
    public static readonly string RedisConnectionString =
        "REDIS_CONNECTION_STRING"
            .RequiredEnvironmentValue;

    public static readonly string? RedisKeyPrefix = "REDIS_KEY_PREFIX"
        .EnvironmentValue;
}