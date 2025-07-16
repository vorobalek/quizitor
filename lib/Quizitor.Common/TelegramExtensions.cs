using Telegram.Bot.Types;

namespace Quizitor.Common;

public static class TelegramExtensions
{
    private static User? TryGetUser(this Update update)
    {
        return
            update.Message?.From
            ?? update.EditedMessage?.From
            ?? update.CallbackQuery?.From;
    }

    public static User GetUser(this Update update)
    {
        return TryGetUser(update) ?? throw new InvalidOperationException("Unable to determine update initiator");
    }
}