using Quizitor.Common;

namespace Quizitor.Bots.Configuration;

public static class AppConfiguration
{
    public static readonly string Locale = "LOCALE"
        .GetEnvironmentValueWithFallback("en");

    public static readonly string Port = "PORT"
        .RequiredEnvironmentValue;

    public static readonly string DbConnectionString = "DB_CONNECTION_STRING"
        .RequiredEnvironmentValue;

    public static readonly string WorkingDirectory = "WORKING_DIRECTORY"
        .GetEnvironmentValueWithFallback("/var/quizitor");

    public static readonly string CryptoPassword = "CRYPTO_PASSWORD"
        .RequiredEnvironmentValue;

    public static readonly string QrCodeExpirationSeconds = "QR_CODE_EXPIRATION_SECONDS"
        .GetEnvironmentValueWithFallback("0");
}