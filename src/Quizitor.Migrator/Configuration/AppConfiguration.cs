using Quizitor.Common;

namespace Quizitor.Migrator.Configuration;

public static class AppConfiguration
{
    public static readonly string? DbConnectionString = "DB_CONNECTION_STRING"
        .GetEnvironmentVariable();

    public static readonly string Locale = "LOCALE"
        .GetEnvironmentVariableWithFallbackValue("en");
}