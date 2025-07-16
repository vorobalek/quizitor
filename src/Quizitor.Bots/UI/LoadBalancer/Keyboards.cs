using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.UI.LoadBalancer;

internal static class Keyboards
{
    public static InlineKeyboardMarkup GameServerRedirect(string username)
    {
        return new InlineKeyboardMarkup([[Buttons.GameServerRedirect(username)]]);
    }

    public static InlineKeyboardMarkup GameAdminRedirect(string username)
    {
        return new InlineKeyboardMarkup([[Buttons.GameAdminRedirect(username)]]);
    }
}