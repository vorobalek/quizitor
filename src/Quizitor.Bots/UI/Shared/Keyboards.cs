using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.UI.Shared;

public static class Keyboards
{
    public static readonly InlineKeyboardMarkup CancelPrompt =
        new([[Buttons.CancelPrompt]]);
}