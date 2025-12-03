using Quizitor.Common;

namespace Quizitor.Api.Configuration;

public static class AppConfiguration
{
    public static readonly string Locale = "LOCALE"
        .GetEnvironmentValueWithFallback("en");

    public static readonly string Port = "PORT"
        .GetEnvironmentValueWithFallback("8080");

    public static readonly string? PathBase = "PATH_BASE"
        .EnvironmentValue;

    public static readonly string DbConnectionString = "DB_CONNECTION_STRING"
        .RequiredEnvironmentValue;
}