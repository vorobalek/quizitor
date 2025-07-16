using Microsoft.Extensions.Options;
using Quizitor.Api.Options;
using Quizitor.Api.Services.Kafka.SendChatAction;
using Quizitor.Api.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Telegram.Bot;

namespace Quizitor.Api.Services.TelegramBot.ClientFactory;

internal sealed class TelegramBotClientFactory(
    IOptions<TelegramBotOptions> options,
    IHttpClientFactory httpClientFactory,
    ISendChatActionKafkaProducer sendChatActionKafkaProducer) :
    ITelegramBotClientFactory
{
    public ITelegramBotClientWrapper CreateForBot(Bot bot)
    {
        return new TelegramBotClientWrapper(
            bot.Id,
            new TelegramBotClient(
                bot.Token,
                httpClientFactory.CreateClient(bot.Id.ToString())),
            sendChatActionKafkaProducer);
    }

    public ITelegramBotClientWrapper CreateDefault()
    {
        return new TelegramBotClientWrapper(
            null,
            new TelegramBotClient(
                options.Value.Token,
                httpClientFactory.CreateClient("BackOffice")),
            sendChatActionKafkaProducer);
    }
}