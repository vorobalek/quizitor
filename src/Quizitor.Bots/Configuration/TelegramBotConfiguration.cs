using Quizitor.Common;

namespace Quizitor.Bots.Configuration;

public static class TelegramBotConfiguration
{
    public static readonly string BotToken = "TELEGRAM_BOT_TOKEN"
        .GetEnvironmentVariableOrThrowIfNullOrWhiteSpace();

    public static readonly bool IsSaUserAuthorizationEnabled = "AUTHORIZED_USER_IDS"
        .GetEnvironmentVariable() != "*";

    public static readonly long[] AuthorizedUserIds = "AUTHORIZED_USER_IDS"
        .GetEnvironmentVariableOrThrowIfNullOrWhiteSpace()
        .Trim()
        .Split(',', ';', ' ')
        .Select(x => x.Trim())
        .Where(x => long.TryParse(x, out _))
        .Select(long.Parse)
        .ToArray();
}