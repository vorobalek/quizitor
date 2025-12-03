using Telegram.Bot.Types;

namespace Quizitor.Common;

public static class TelegramExtensions
{
    extension(Update update)
    {
        private User? TryGetUser()
        {
            return
                update.Message?.From
                ?? update.EditedMessage?.From
                ?? update.CallbackQuery?.From;
        }

        public User GetUser()
        {
            return TryGetUser(update) ?? throw new InvalidOperationException("Unable to determine update initiator");
        }
    }
}