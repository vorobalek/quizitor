using Quizitor.Common;

namespace Quizitor.Sender.Configuration;

public static class TelegramBotConfiguration
{
    public static readonly string BotToken = "TELEGRAM_BOT_TOKEN"
        .RequiredEnvironmentValue;
}