using Microsoft.AspNetCore.Mvc;
using Moq;
using Quizitor.Api.Controllers;
using Quizitor.Api.Services.Kafka.Updates;
using Quizitor.Api.Services.TelegramBot.ClientFactory;
using Quizitor.Api.Services.TelegramBot.ClientWrapper;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Data.Repositories;
using Quizitor.Kafka.Contracts;
using Quizitor.Tests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Api.Tests.Controllers;

[TestClass]
public class WebhookControllerTests
{
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task Post_Message_Produces_OkResult(bool isTest)
    {
        var updateId = Unique.Int32();
        var chatId = Unique.Int64();
        var update = new Update
        {
            Id = updateId,
            Message = new Message
            {
                Chat = new Chat
                {
                    Id = chatId
                }
            }
        };

        var cancellationToken = CancellationToken.None;
        var globalCancellationTokenSourceMock = new Mock<IGlobalCancellationTokenSource>(MockBehavior.Strict);
        globalCancellationTokenSourceMock
            .SetupGet(x => x.Token)
            .Returns(cancellationToken)
            .Verifiable(Times.Once);

        var dateTimeOffset = Unique.DateTimeOffset();
        var dbContextProviderMock = new Mock<IDbContextProvider>(MockBehavior.Strict);
        dbContextProviderMock
            .SetupGet(x => x.Bots)
            .Verifiable(Times.Never);
        dbContextProviderMock
            .Setup(x => x.GetServerDateTimeOffsetAsync(cancellationToken))
            .ReturnsAsync(dateTimeOffset)
            .Verifiable(Times.Once);

        var telegramBotClientWrapperMock = new Mock<ITelegramBotClientWrapper>(MockBehavior.Strict);
        telegramBotClientWrapperMock
            .Setup(x => x.SendChatAction(
                It.Is<UpdateContext>(e =>
                    e.Update == update &&
                    e.InitiatedAt == dateTimeOffset &&
                    e.IsTest == isTest),
                It.Is<ChatId>(e => e.Identifier == chatId),
                ChatAction.Typing,
                null,
                null,
                cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        var telegramBotClientFactoryMock = new Mock<ITelegramBotClientFactory>(MockBehavior.Strict);
        telegramBotClientFactoryMock
            .Setup(x => x.CreateDefault())
            .Returns(telegramBotClientWrapperMock.Object)
            .Verifiable(Times.Once);

        var updateKafkaProducerMock = new Mock<IUpdateKafkaProducer>(MockBehavior.Strict);
        updateKafkaProducerMock
            .Setup(x => x.ProduceAsync(
                update,
                dateTimeOffset,
                isTest,
                cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);


        var result = await new WebhookController(
                dbContextProviderMock.Object,
                telegramBotClientFactoryMock.Object,
                updateKafkaProducerMock.Object,
                globalCancellationTokenSourceMock.Object)
            .Post(update, isTest);


        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<OkResult>(result);

        dbContextProviderMock.VerifyAll();
        dbContextProviderMock.VerifyNoOtherCalls();

        globalCancellationTokenSourceMock.VerifyAll();
        globalCancellationTokenSourceMock.VerifyNoOtherCalls();

        telegramBotClientWrapperMock.VerifyAll();
        telegramBotClientWrapperMock.VerifyNoOtherCalls();

        telegramBotClientFactoryMock.VerifyAll();
        telegramBotClientFactoryMock.VerifyNoOtherCalls();

        updateKafkaProducerMock.VerifyAll();
        updateKafkaProducerMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task Post_CallbackQuery_Produces_OkResult(bool isTest)
    {
        var updateId = Unique.Int32();
        var update = new Update
        {
            Id = updateId,
            CallbackQuery = new CallbackQuery()
        };

        var cancellationToken = CancellationToken.None;
        var globalCancellationTokenSourceMock = new Mock<IGlobalCancellationTokenSource>(MockBehavior.Strict);
        globalCancellationTokenSourceMock
            .SetupGet(x => x.Token)
            .Returns(cancellationToken)
            .Verifiable(Times.Once);

        var dateTimeOffset = Unique.DateTimeOffset();
        var dbContextProviderMock = new Mock<IDbContextProvider>(MockBehavior.Strict);
        dbContextProviderMock
            .SetupGet(x => x.Bots)
            .Verifiable(Times.Never);
        dbContextProviderMock
            .Setup(x => x.GetServerDateTimeOffsetAsync(cancellationToken))
            .ReturnsAsync(dateTimeOffset)
            .Verifiable(Times.Once);

        var telegramBotClientFactoryMock = new Mock<ITelegramBotClientFactory>(MockBehavior.Strict);
        telegramBotClientFactoryMock
            .Setup(x => x.CreateDefault())
            .Verifiable(Times.Never);
        telegramBotClientFactoryMock
            .Setup(x => x.CreateForBot(It.IsAny<Bot>()))
            .Verifiable(Times.Never);

        var updateKafkaProducerMock = new Mock<IUpdateKafkaProducer>(MockBehavior.Strict);
        updateKafkaProducerMock
            .Setup(x => x.ProduceAsync(
                update,
                dateTimeOffset,
                isTest,
                cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);


        var result = await new WebhookController(
                dbContextProviderMock.Object,
                telegramBotClientFactoryMock.Object,
                updateKafkaProducerMock.Object,
                globalCancellationTokenSourceMock.Object)
            .Post(update, isTest);


        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<OkResult>(result);

        dbContextProviderMock.VerifyAll();
        dbContextProviderMock.VerifyNoOtherCalls();

        globalCancellationTokenSourceMock.VerifyAll();
        globalCancellationTokenSourceMock.VerifyNoOtherCalls();

        telegramBotClientFactoryMock.VerifyAll();
        telegramBotClientFactoryMock.VerifyNoOtherCalls();

        updateKafkaProducerMock.VerifyAll();
        updateKafkaProducerMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task PostBotId_Message_Produces_OkResult(bool isTest)
    {
        var updateId = Unique.Int32();
        var botId = Unique.Int32();
        var chatId = Unique.Int64();
        var update = new Update
        {
            Id = updateId,
            Message = new Message
            {
                Chat = new Chat
                {
                    Id = chatId
                }
            }
        };
        var bot = new Bot
        {
            Id = botId,
            IsActive = true
        };

        var cancellationToken = CancellationToken.None;
        var globalCancellationTokenSourceMock = new Mock<IGlobalCancellationTokenSource>(MockBehavior.Strict);
        globalCancellationTokenSourceMock
            .SetupGet(x => x.Token)
            .Returns(cancellationToken)
            .Verifiable(Times.Once);

        var botRepositoryMock = new Mock<IBotRepository>(MockBehavior.Strict);
        botRepositoryMock
            .Setup(x => x.GetByIdOrDefaultAsync(botId, cancellationToken))
            .ReturnsAsync(bot)
            .Verifiable(Times.Once);

        var dateTimeOffset = Unique.DateTimeOffset();
        var dbContextProviderMock = new Mock<IDbContextProvider>(MockBehavior.Strict);
        dbContextProviderMock
            .SetupGet(x => x.Bots)
            .Returns(botRepositoryMock.Object)
            .Verifiable(Times.Once);
        dbContextProviderMock
            .Setup(x => x.GetServerDateTimeOffsetAsync(cancellationToken))
            .ReturnsAsync(dateTimeOffset)
            .Verifiable(Times.Once);

        var telegramBotClientWrapperMock = new Mock<ITelegramBotClientWrapper>(MockBehavior.Strict);
        telegramBotClientWrapperMock
            .Setup(x => x.SendChatAction(
                It.Is<UpdateContext>(e =>
                    e.Update == update &&
                    e.InitiatedAt == dateTimeOffset &&
                    e.IsTest == isTest),
                It.Is<ChatId>(e => e.Identifier == chatId),
                ChatAction.Typing,
                null,
                null,
                cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        var telegramBotClientFactoryMock = new Mock<ITelegramBotClientFactory>(MockBehavior.Strict);
        telegramBotClientFactoryMock
            .Setup(x => x.CreateForBot(bot))
            .Returns(telegramBotClientWrapperMock.Object)
            .Verifiable(Times.Once);

        var updateKafkaProducerMock = new Mock<IUpdateKafkaProducer>(MockBehavior.Strict);
        updateKafkaProducerMock
            .Setup(x => x.ProduceBotAsync(
                botId,
                update,
                dateTimeOffset,
                isTest,
                cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);


        var result = await new WebhookController(
                dbContextProviderMock.Object,
                telegramBotClientFactoryMock.Object,
                updateKafkaProducerMock.Object,
                globalCancellationTokenSourceMock.Object)
            .Post(botId, update, isTest);


        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<OkResult>(result);

        dbContextProviderMock.VerifyAll();
        dbContextProviderMock.VerifyNoOtherCalls();

        botRepositoryMock.VerifyAll();
        botRepositoryMock.VerifyNoOtherCalls();

        globalCancellationTokenSourceMock.VerifyAll();
        globalCancellationTokenSourceMock.VerifyNoOtherCalls();

        telegramBotClientWrapperMock.VerifyAll();
        telegramBotClientWrapperMock.VerifyNoOtherCalls();

        telegramBotClientFactoryMock.VerifyAll();
        telegramBotClientFactoryMock.VerifyNoOtherCalls();

        updateKafkaProducerMock.VerifyAll();
        updateKafkaProducerMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task PostBotId_CallbackQuery_Produces_OkResult(bool isTest)
    {
        var updateId = Unique.Int32();
        var botId = Unique.Int32();
        var update = new Update
        {
            Id = updateId,
            CallbackQuery = new CallbackQuery()
        };

        var cancellationToken = CancellationToken.None;
        var globalCancellationTokenSourceMock = new Mock<IGlobalCancellationTokenSource>(MockBehavior.Strict);
        globalCancellationTokenSourceMock
            .SetupGet(x => x.Token)
            .Returns(cancellationToken)
            .Verifiable(Times.Once);

        var botRepositoryMock = new Mock<IBotRepository>(MockBehavior.Strict);
        botRepositoryMock
            .Setup(x => x.GetByIdOrDefaultAsync(botId, cancellationToken))
            .ReturnsAsync(new Bot
            {
                Id = botId,
                IsActive = true
            })
            .Verifiable(Times.Once);

        var dateTimeOffset = Unique.DateTimeOffset();
        var dbContextProviderMock = new Mock<IDbContextProvider>(MockBehavior.Strict);
        dbContextProviderMock
            .SetupGet(x => x.Bots)
            .Returns(botRepositoryMock.Object)
            .Verifiable(Times.Once);
        dbContextProviderMock
            .Setup(x => x.GetServerDateTimeOffsetAsync(cancellationToken))
            .ReturnsAsync(dateTimeOffset)
            .Verifiable(Times.Once);

        var telegramBotClientFactoryMock = new Mock<ITelegramBotClientFactory>(MockBehavior.Strict);
        telegramBotClientFactoryMock
            .Setup(x => x.CreateDefault())
            .Verifiable(Times.Never);
        telegramBotClientFactoryMock
            .Setup(x => x.CreateForBot(It.IsAny<Bot>()))
            .Verifiable(Times.Never);

        var updateKafkaProducerMock = new Mock<IUpdateKafkaProducer>(MockBehavior.Strict);
        updateKafkaProducerMock
            .Setup(x => x.ProduceBotAsync(
                botId,
                update,
                dateTimeOffset,
                isTest,
                cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);


        var result = await new WebhookController(
                dbContextProviderMock.Object,
                telegramBotClientFactoryMock.Object,
                updateKafkaProducerMock.Object,
                globalCancellationTokenSourceMock.Object)
            .Post(botId, update, isTest);


        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<OkResult>(result);

        dbContextProviderMock.VerifyAll();
        dbContextProviderMock.VerifyNoOtherCalls();

        botRepositoryMock.VerifyAll();
        botRepositoryMock.VerifyNoOtherCalls();

        globalCancellationTokenSourceMock.VerifyAll();
        globalCancellationTokenSourceMock.VerifyNoOtherCalls();

        telegramBotClientFactoryMock.VerifyAll();
        telegramBotClientFactoryMock.VerifyNoOtherCalls();

        updateKafkaProducerMock.VerifyAll();
        updateKafkaProducerMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task PostBotId_MessageWhenBotIsNull_SkipNotFoundResult()
    {
        var botId = Unique.Int32();
        var chatId = Unique.Int64();
        var update = new Update
        {
            Message = new Message
            {
                Chat = new Chat
                {
                    Id = chatId
                }
            }
        };

        var cancellationToken = CancellationToken.None;
        var globalCancellationTokenSourceMock = new Mock<IGlobalCancellationTokenSource>(MockBehavior.Strict);
        globalCancellationTokenSourceMock
            .SetupGet(x => x.Token)
            .Returns(cancellationToken)
            .Verifiable(Times.Once);

        var botRepositoryMock = new Mock<IBotRepository>(MockBehavior.Strict);
        botRepositoryMock
            .Setup(x => x.GetByIdOrDefaultAsync(botId, cancellationToken))
            .ReturnsAsync(default(Bot))
            .Verifiable(Times.Once);

        var dbContextProviderMock = new Mock<IDbContextProvider>(MockBehavior.Strict);
        dbContextProviderMock
            .SetupGet(x => x.Bots)
            .Returns(botRepositoryMock.Object)
            .Verifiable(Times.Once);
        dbContextProviderMock
            .Setup(x => x.GetServerDateTimeOffsetAsync(cancellationToken))
            .ReturnsAsync(Unique.DateTimeOffset())
            .Verifiable(Times.Once);

        var telegramBotClientFactoryMock = new Mock<ITelegramBotClientFactory>(MockBehavior.Strict);
        telegramBotClientFactoryMock
            .Setup(x => x.CreateDefault())
            .Verifiable(Times.Never);
        telegramBotClientFactoryMock
            .Setup(x => x.CreateForBot(It.IsAny<Bot>()))
            .Verifiable(Times.Never);

        var updateKafkaProducerMock = new Mock<IUpdateKafkaProducer>(MockBehavior.Strict);
        updateKafkaProducerMock
            .Setup(x => x.ProduceBotAsync(
                It.IsAny<int>(),
                It.IsAny<Update>(),
                It.IsAny<DateTimeOffset>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Verifiable(Times.Never);


        var result = await new WebhookController(
                dbContextProviderMock.Object,
                telegramBotClientFactoryMock.Object,
                updateKafkaProducerMock.Object,
                globalCancellationTokenSourceMock.Object)
            .Post(botId, update);


        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<NotFoundResult>(result);

        dbContextProviderMock.VerifyAll();
        dbContextProviderMock.VerifyNoOtherCalls();

        botRepositoryMock.VerifyAll();
        botRepositoryMock.VerifyNoOtherCalls();

        globalCancellationTokenSourceMock.VerifyAll();
        globalCancellationTokenSourceMock.VerifyNoOtherCalls();

        telegramBotClientFactoryMock.VerifyAll();
        telegramBotClientFactoryMock.VerifyNoOtherCalls();

        updateKafkaProducerMock.VerifyAll();
        updateKafkaProducerMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task PostBotId_MessageWhenBotIsNotActiveAndDropPendingUpdatesFalse_SkipNotFoundResult(bool isTest)
    {
        var botId = Unique.Int32();
        var bot = new Bot
        {
            Id = botId,
            IsActive = false,
            DropPendingUpdates = false
        };
        var chatId = Unique.Int64();
        var update = new Update
        {
            Message = new Message
            {
                Chat = new Chat
                {
                    Id = chatId
                }
            }
        };
        var dateTimeOffset = Unique.DateTimeOffset();

        var cancellationToken = CancellationToken.None;
        var globalCancellationTokenSourceMock = new Mock<IGlobalCancellationTokenSource>(MockBehavior.Strict);
        globalCancellationTokenSourceMock
            .SetupGet(x => x.Token)
            .Returns(cancellationToken)
            .Verifiable(Times.Once);

        var botRepositoryMock = new Mock<IBotRepository>(MockBehavior.Strict);
        botRepositoryMock
            .Setup(x => x.GetByIdOrDefaultAsync(botId, cancellationToken))
            .ReturnsAsync(bot)
            .Verifiable(Times.Once);

        var dbContextProviderMock = new Mock<IDbContextProvider>(MockBehavior.Strict);
        dbContextProviderMock
            .SetupGet(x => x.Bots)
            .Returns(botRepositoryMock.Object)
            .Verifiable(Times.Once);
        dbContextProviderMock
            .Setup(x => x.GetServerDateTimeOffsetAsync(cancellationToken))
            .ReturnsAsync(dateTimeOffset)
            .Verifiable(Times.Once);

        var telegramBotClientFactoryMock = new Mock<ITelegramBotClientFactory>(MockBehavior.Strict);
        telegramBotClientFactoryMock
            .Setup(x => x.CreateDefault())
            .Verifiable(Times.Never);
        telegramBotClientFactoryMock
            .Setup(x => x.CreateForBot(It.IsAny<Bot>()))
            .Verifiable(Times.Never);

        var updateKafkaProducerMock = new Mock<IUpdateKafkaProducer>(MockBehavior.Strict);
        updateKafkaProducerMock
            .Setup(x => x.ProduceBotAsync(
                botId,
                update,
                null,
                isTest,
                cancellationToken))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);


        var result = await new WebhookController(
                dbContextProviderMock.Object,
                telegramBotClientFactoryMock.Object,
                updateKafkaProducerMock.Object,
                globalCancellationTokenSourceMock.Object)
            .Post(botId, update, isTest);


        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<OkResult>(result);

        dbContextProviderMock.VerifyAll();
        dbContextProviderMock.VerifyNoOtherCalls();

        botRepositoryMock.VerifyAll();
        botRepositoryMock.VerifyNoOtherCalls();

        globalCancellationTokenSourceMock.VerifyAll();
        globalCancellationTokenSourceMock.VerifyNoOtherCalls();

        telegramBotClientFactoryMock.VerifyAll();
        telegramBotClientFactoryMock.VerifyNoOtherCalls();

        updateKafkaProducerMock.VerifyAll();
        updateKafkaProducerMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task PostBotId_MessageWhenBotIsNotActiveAndDropPendingUpdatesTrue_SkipNotFoundResult(bool isTest)
    {
        var botId = Unique.Int32();
        var bot = new Bot
        {
            Id = botId,
            IsActive = false,
            DropPendingUpdates = true
        };
        var chatId = Unique.Int64();
        var update = new Update
        {
            Message = new Message
            {
                Chat = new Chat
                {
                    Id = chatId
                }
            }
        };
        var dateTimeOffset = Unique.DateTimeOffset();

        var cancellationToken = CancellationToken.None;
        var globalCancellationTokenSourceMock = new Mock<IGlobalCancellationTokenSource>(MockBehavior.Strict);
        globalCancellationTokenSourceMock
            .SetupGet(x => x.Token)
            .Returns(cancellationToken)
            .Verifiable(Times.Once);

        var botRepositoryMock = new Mock<IBotRepository>(MockBehavior.Strict);
        botRepositoryMock
            .Setup(x => x.GetByIdOrDefaultAsync(botId, cancellationToken))
            .ReturnsAsync(bot)
            .Verifiable(Times.Once);

        var dbContextProviderMock = new Mock<IDbContextProvider>(MockBehavior.Strict);
        dbContextProviderMock
            .SetupGet(x => x.Bots)
            .Returns(botRepositoryMock.Object)
            .Verifiable(Times.Once);
        dbContextProviderMock
            .Setup(x => x.GetServerDateTimeOffsetAsync(cancellationToken))
            .ReturnsAsync(dateTimeOffset)
            .Verifiable(Times.Once);

        var telegramBotClientFactoryMock = new Mock<ITelegramBotClientFactory>(MockBehavior.Strict);
        telegramBotClientFactoryMock
            .Setup(x => x.CreateDefault())
            .Verifiable(Times.Never);
        telegramBotClientFactoryMock
            .Setup(x => x.CreateForBot(It.IsAny<Bot>()))
            .Verifiable(Times.Never);

        var updateKafkaProducerMock = new Mock<IUpdateKafkaProducer>(MockBehavior.Strict);
        updateKafkaProducerMock
            .Setup(x => x.ProduceBotAsync(
                It.IsAny<int>(),
                It.IsAny<Update>(),
                It.IsAny<DateTimeOffset>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .Verifiable(Times.Never);


        var result = await new WebhookController(
                dbContextProviderMock.Object,
                telegramBotClientFactoryMock.Object,
                updateKafkaProducerMock.Object,
                globalCancellationTokenSourceMock.Object)
            .Post(botId, update, isTest);


        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<OkResult>(result);

        dbContextProviderMock.VerifyAll();
        dbContextProviderMock.VerifyNoOtherCalls();

        botRepositoryMock.VerifyAll();
        botRepositoryMock.VerifyNoOtherCalls();

        globalCancellationTokenSourceMock.VerifyAll();
        globalCancellationTokenSourceMock.VerifyNoOtherCalls();

        telegramBotClientFactoryMock.VerifyAll();
        telegramBotClientFactoryMock.VerifyNoOtherCalls();

        updateKafkaProducerMock.VerifyAll();
        updateKafkaProducerMock.VerifyNoOtherCalls();
    }
}