using Quizitor.Common;

namespace Quizitor.Bots.Configuration;

public static class AppConfiguration
{
    public static readonly string Locale = "LOCALE"
        .GetEnvironmentVariableWithFallbackValue("en");

    public static readonly string Port = "PORT"
        .GetEnvironmentVariableOrThrowIfNullOrWhiteSpace();

    public static readonly string? DbConnectionString = "DB_CONNECTION_STRING"
        .GetEnvironmentVariable();

    public static readonly string WorkingDirectory = "WORKING_DIRECTORY"
        .GetEnvironmentVariableWithFallbackValue("/var/quizitor");

    public static readonly string CryptoPassword = "CRYPTO_PASSWORD"
        .GetEnvironmentVariableOrThrowIfNullOrWhiteSpace();

    public static readonly string QrCodeExpirationSeconds = "QR_CODE_EXPIRATION_SECONDS"
        .GetEnvironmentVariableWithFallbackValue("0");
}