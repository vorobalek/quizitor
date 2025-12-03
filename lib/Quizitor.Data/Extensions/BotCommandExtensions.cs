using Quizitor.Data.Entities;
using TelegramBotCommand = Telegram.Bot.Types.BotCommand;

namespace Quizitor.Data.Extensions;

public static class BotCommandExtensions
{
    extension(BotCommand botCommand)
    {
        public TelegramBotCommand TelegramBotCommand =>
            new()
            {
                Command = botCommand.Command,
                Description = botCommand.Description
            };
    }
}