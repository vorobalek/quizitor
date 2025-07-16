using Quizitor.Common;

namespace Quizitor.Events.Configuration;

public static class AppConfiguration
{
    public static readonly string Port = "PORT"
        .GetEnvironmentVariableOrThrowIfNullOrWhiteSpace();

    public static readonly string? DbConnectionString = "DB_CONNECTION_STRING"
        .GetEnvironmentVariable();
}