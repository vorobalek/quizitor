using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;

namespace Quizitor.Bots.Services.TelegramBot.ClientFactory;

internal interface ITelegramBotClientFactory
{
    ITelegramBotClientWrapper CreateForBot(Bot bot);
    ITelegramBotClientWrapper CreateDefault();
}