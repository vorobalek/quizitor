using Quizitor.Bots.Configuration;
using Quizitor.Bots.Services.Kafka.Producers.AnswerCallbackQuery;
using Quizitor.Bots.Services.Kafka.Producers.DeleteMessage;
using Quizitor.Bots.Services.Kafka.Producers.EditMessage;
using Quizitor.Bots.Services.Kafka.Producers.SendChatAction;
using Quizitor.Bots.Services.Kafka.Producers.SendMessage;
using Quizitor.Bots.Services.Kafka.Producers.SendPhoto;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Telegram.Bot;

namespace Quizitor.Bots.Services.TelegramBot.ClientFactory;

internal sealed class TelegramBotClientFactory(
    IHttpClientFactory httpClientFactory,
    ISendMessageKafkaProducer sendMessageKafkaProducer,
    IEditMessageKafkaProducer editMessageKafkaProducer,
    IAnswerCallbackQueryKafkaProducer answerCallbackQueryKafkaProducer,
    IDeleteMessageKafkaProducer deleteMessageKafkaProducer,
    ISendChatActionKafkaProducer sendChatActionKafkaProducer,
    ISendPhotoKafkaProducer sendPhotoKafkaProducer) : ITelegramBotClientFactory
{
    public ITelegramBotClientWrapper CreateForBot(Bot bot)
    {
        return new TelegramBotClientWrapper(
            bot.Id,
            new TelegramBotClient(
                bot.Token,
                httpClientFactory.CreateClient(bot.Id.ToString())),
            sendMessageKafkaProducer,
            editMessageKafkaProducer,
            answerCallbackQueryKafkaProducer,
            deleteMessageKafkaProducer,
            sendChatActionKafkaProducer,
            sendPhotoKafkaProducer);
    }

    public ITelegramBotClientWrapper CreateDefault()
    {
        return new TelegramBotClientWrapper(
            null,
            new TelegramBotClient(
                TelegramBotConfiguration.BotToken,
                httpClientFactory.CreateClient("BackOffice")),
            sendMessageKafkaProducer,
            editMessageKafkaProducer,
            answerCallbackQueryKafkaProducer,
            deleteMessageKafkaProducer,
            sendChatActionKafkaProducer,
            sendPhotoKafkaProducer);
    }
}