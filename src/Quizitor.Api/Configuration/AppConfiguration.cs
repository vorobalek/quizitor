using Quizitor.Common;

namespace Quizitor.Api.Configuration;

public static class AppConfiguration
{
    public static readonly string Locale = "LOCALE"
        .GetEnvironmentVariableWithFallbackValue("en");

    public static readonly string Port = "PORT"
        .GetEnvironmentVariableWithFallbackValue("8080");

    public static readonly string? PathBase = "PATH_BASE"
        .GetEnvironmentVariable();

    public static readonly string? DbConnectionString = "DB_CONNECTION_STRING"
        .GetEnvironmentVariable();
}