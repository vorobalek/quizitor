using Microsoft.Extensions.Options;
using Moq;
using Quizitor.Api.Options;
using Quizitor.Api.Services.Kafka.SendChatAction;
using Quizitor.Api.Services.TelegramBot.ClientFactory;
using Quizitor.Data.Entities;
using Quizitor.Tests;

namespace Quizitor.Api.Tests.Services.TelegramBot.ClientFactory;

[TestClass]
public class TelegramBotClientFactoryTests
{
    [TestMethod]
    public void Create_Default_HappyPath()
    {
        var token = Unique.TelegramBotToken();

        var httpClientFactoryMock = new Mock<IHttpClientFactory>(MockBehavior.Strict);
        httpClientFactoryMock
            .Setup(x => x.CreateClient("BackOffice"))
            .Returns(new HttpClient());

        var sendChatActionKafkaProducerMock = new Mock<ISendChatActionKafkaProducer>(MockBehavior.Strict);


        var result = new TelegramBotClientFactory(
                new OptionsWrapper<TelegramBotOptions>(
                    new TelegramBotOptions
                    {
                        Token = token
                    }),
                httpClientFactoryMock.Object,
                sendChatActionKafkaProducerMock.Object)
            .CreateDefault();


        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Create_ForBot_HappyPath()
    {
        var botId = Unique.Int32();
        var botToken = Unique.TelegramBotToken();
        var bot = new Bot
        {
            Id = botId,
            Token = botToken
        };
        var token = Unique.TelegramBotToken();
        var httpClientFactoryMock = new Mock<IHttpClientFactory>(MockBehavior.Strict);
        httpClientFactoryMock
            .Setup(x => x.CreateClient(botId.ToString()))
            .Returns(new HttpClient());

        var sendChatActionKafkaProducerMock = new Mock<ISendChatActionKafkaProducer>(MockBehavior.Strict);


        var result = new TelegramBotClientFactory(
                new OptionsWrapper<TelegramBotOptions>(
                    new TelegramBotOptions
                    {
                        Token = token
                    }),
                httpClientFactoryMock.Object,
                sendChatActionKafkaProducerMock.Object)
            .CreateForBot(bot);


        Assert.IsNotNull(result);
    }
}