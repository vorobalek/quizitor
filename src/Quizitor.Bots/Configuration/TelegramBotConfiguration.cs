using Quizitor.Common;

namespace Quizitor.Bots.Configuration;

public static class TelegramBotConfiguration
{
    public static readonly string BotToken = "TELEGRAM_BOT_TOKEN"
        .RequiredEnvironmentValue;

    public static readonly bool IsSaUserAuthorizationEnabled = "AUTHORIZED_USER_IDS"
        .EnvironmentValue != "*";

    public static readonly long[] AuthorizedUserIds =
    [
        .. "AUTHORIZED_USER_IDS"
            .RequiredEnvironmentValue
            .Trim()
            .Split(',', ';', ' ')
            .Select(x => x.Trim())
            .Where(x => long.TryParse(x, out _))
            .Select(long.Parse)
    ];
}