using Quizitor.Common;

namespace Quizitor.Sender.Configuration;

public static class AppConfiguration
{
    public static readonly string Port = "PORT"
        .GetEnvironmentVariableOrThrowIfNullOrWhiteSpace();

    public static readonly string? DbConnectionString = "DB_CONNECTION_STRING"
        .GetEnvironmentVariable();

    public static readonly string WorkingDirectory = "WORKING_DIRECTORY"
        .GetEnvironmentVariableWithFallbackValue("/var/quizitor");
}