using Quizitor.Common;

namespace Quizitor.Sender.Configuration;

public static class AppConfiguration
{
    public static readonly string Port = "PORT"
        .RequiredEnvironmentValue;

    public static readonly string DbConnectionString = "DB_CONNECTION_STRING"
        .RequiredEnvironmentValue;

    public static readonly string WorkingDirectory = "WORKING_DIRECTORY"
        .GetEnvironmentValueWithFallback("/var/quizitor");
}