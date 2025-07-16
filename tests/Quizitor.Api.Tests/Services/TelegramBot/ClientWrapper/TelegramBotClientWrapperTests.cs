using Moq;
using Quizitor.Api.Services.Kafka.SendChatAction;
using Quizitor.Api.Services.TelegramBot.ClientWrapper;
using Quizitor.Kafka.Contracts;
using Quizitor.Tests;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Api.Tests.Services.TelegramBot.ClientWrapper;

[TestClass]
public class TelegramBotClientWrapperTests
{
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task SendChatAction_HappyPath(bool isTest)
    {
        var updateContext = new UpdateContext(
            null,
            new Update
            {
                Id = Unique.Int32()
            },
            Unique.DateTimeOffset(),
            isTest);
        var chatId = new ChatId(Unique.Int64());
        var chatAction = Unique.Enum<ChatAction>();
        var messageThreadId = Unique.Int32();
        var businessConnectionId = Unique.String();
        var cancellationToken = CancellationToken.None;

        var telegramBotClientMock = new Mock<ITelegramBotClient>(MockBehavior.Strict);

        var sendChatActionKafkaProducerMock = new Mock<ISendChatActionKafkaProducer>(MockBehavior.Strict);
        sendChatActionKafkaProducerMock
            .Setup(x => x.ProduceAsync(
                It.Is<SendChatActionRequest>(e =>
                    e.ChatId == chatId &&
                    e.Action == chatAction &&
                    e.MessageThreadId == messageThreadId &&
                    e.BusinessConnectionId == businessConnectionId),
                updateContext,
                cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);


        await new TelegramBotClientWrapper(
                null,
                telegramBotClientMock.Object,
                sendChatActionKafkaProducerMock.Object)
            .SendChatAction(
                updateContext,
                chatId,
                chatAction,
                messageThreadId,
                businessConnectionId,
                cancellationToken);


        telegramBotClientMock.VerifyAll();
        telegramBotClientMock.VerifyNoOtherCalls();

        sendChatActionKafkaProducerMock.VerifyAll();
        sendChatActionKafkaProducerMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task SendChatAction_WithBotId_HappyPath(bool isTest)
    {
        var botId = Unique.Int32();
        var updateContext = new UpdateContext(
            botId,
            new Update
            {
                Id = Unique.Int32()
            },
            Unique.DateTimeOffset(),
            isTest);
        var chatId = new ChatId(Unique.Int64());
        var chatAction = Unique.Enum<ChatAction>();
        var messageThreadId = Unique.Int32();
        var businessConnectionId = Unique.String();
        var cancellationToken = CancellationToken.None;

        var telegramBotClientMock = new Mock<ITelegramBotClient>(MockBehavior.Strict);

        var sendChatActionKafkaProducerMock = new Mock<ISendChatActionKafkaProducer>(MockBehavior.Strict);
        sendChatActionKafkaProducerMock
            .Setup(x => x.ProduceBotAsync(
                botId,
                It.Is<SendChatActionRequest>(e =>
                    e.ChatId == chatId &&
                    e.Action == chatAction &&
                    e.MessageThreadId == messageThreadId &&
                    e.BusinessConnectionId == businessConnectionId),
                updateContext,
                cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);


        await new TelegramBotClientWrapper(
                botId,
                telegramBotClientMock.Object,
                sendChatActionKafkaProducerMock.Object)
            .SendChatAction(
                updateContext,
                chatId,
                chatAction,
                messageThreadId,
                businessConnectionId,
                cancellationToken);


        telegramBotClientMock.VerifyAll();
        telegramBotClientMock.VerifyNoOtherCalls();

        sendChatActionKafkaProducerMock.VerifyAll();
        sendChatActionKafkaProducerMock.VerifyNoOtherCalls();
    }
}