using LPlus;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.UI.LoadBalancer;

internal static class Buttons
{
    public static InlineKeyboardButton GameServerRedirect(
        string username)
    {
        return InlineKeyboardButton.WithUrl(
            TR.L + "_LOAD_BALANCER_GAME_SERVER_REDIRECT_BTN",
            $"https://t.me/{username}");
    }

    public static InlineKeyboardButton GameAdminRedirect(
        string username)
    {
        return InlineKeyboardButton.WithUrl(
            TR.L + "_LOAD_BALANCER_GAME_ADMIN_REDIRECT_BTN",
            $"https://t.me/{username}");
    }
}