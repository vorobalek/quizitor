using Telegram.Bot.Types;

namespace Quizitor.Data.Extensions;

public static class BotCommandExtensions
{
    public static BotCommand ToTelegramBotCommand(this Entities.BotCommand botCommand)
    {
        return new BotCommand
        {
            Command = botCommand.Command,
            Description = botCommand.Description
        };
    }
}