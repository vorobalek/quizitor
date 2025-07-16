using Quizitor.Common;

namespace Quizitor.Events.Configuration;

public static class SentryConfiguration
{
    public const LogLevel MinimumEventLevel = LogLevel.Error;

    public static readonly string? Dsn = "SENTRY_DSN"
        .GetEnvironmentVariable();
}