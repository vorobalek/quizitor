using Quizitor.Api.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;

namespace Quizitor.Api.Services.TelegramBot.ClientFactory;

public interface ITelegramBotClientFactory
{
    ITelegramBotClientWrapper CreateForBot(Bot bot);
    ITelegramBotClientWrapper CreateDefault();
}