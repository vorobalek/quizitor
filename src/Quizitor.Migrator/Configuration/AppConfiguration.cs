using Quizitor.Common;

namespace Quizitor.Migrator.Configuration;

public static class AppConfiguration
{
    public static readonly string? DbConnectionString = "DB_CONNECTION_STRING"
        .EnvironmentValue;

    public static readonly string Locale = "LOCALE"
        .GetEnvironmentValueWithFallback("en");
}