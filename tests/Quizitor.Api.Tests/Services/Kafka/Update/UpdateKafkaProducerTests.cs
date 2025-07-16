using System.Text.Json;
using Confluent.Kafka;
using Moq;
using Quizitor.Api.Services.Kafka.Updates;
using Quizitor.Kafka;
using Quizitor.Kafka.Contracts;
using Quizitor.Tests;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Quizitor.Api.Tests.Services.Kafka.Update;

[TestClass]
public class UpdateKafkaProducerTests
{
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task ProduceAsync_HappyPath(bool isTest)
    {
        const string topic = "Quizitor.Update";
        var dateTimeOffset = Unique.DateTimeOffset();
        var updateId = Unique.Int32();
        var chatId = Unique.Int64();
        var update = new Telegram.Bot.Types.Update
        {
            Id = updateId,
            Message = new Message
            {
                From = new User
                {
                    Id = chatId
                },
                Text = Unique.String()
            }
        };
        var cancellationToken = CancellationToken.None;

        var kafkaProducerMock = new Mock<IProducer<long, string>>(MockBehavior.Strict);
        kafkaProducerMock
            .Setup(x => x.ProduceAsync(
                topic,
                It.Is<Message<long, string>>(e =>
                    e.Key == chatId &&
                    e.Value == JsonSerializer.Serialize(
                        new UpdateContext(
                            null,
                            update,
                            dateTimeOffset,
                            isTest),
                        JsonBotAPI.Options)),
                cancellationToken))
            .ReturnsAsync(new DeliveryReport<long, string>())
            .Verifiable(Times.Once);

        var kafkaProducerFactoryMock = new Mock<IKafkaProducerFactory<long, string>>(MockBehavior.Strict);
        kafkaProducerFactoryMock
            .Setup(x => x.Create(topic))
            .Returns(kafkaProducerMock.Object)
            .Verifiable(Times.Once);


        await new UpdateKafkaProducer(kafkaProducerFactoryMock.Object)
            .ProduceAsync(
                update,
                dateTimeOffset,
                isTest,
                cancellationToken);


        kafkaProducerMock.VerifyAll();
        kafkaProducerMock.VerifyNoOtherCalls();

        kafkaProducerFactoryMock.VerifyAll();
        kafkaProducerFactoryMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task ProduceBotAsync_HappyPath(bool isTest)
    {
        var dateTimeOffset = Unique.DateTimeOffset();
        var botId = Unique.Int32();
        var topic = $"Quizitor.Update.{botId}";
        var updateId = Unique.Int32();
        var chatId = Unique.Int64();
        var update = new Telegram.Bot.Types.Update
        {
            Id = updateId,
            Message = new Message
            {
                From = new User
                {
                    Id = chatId
                },
                Text = Unique.String()
            }
        };
        var cancellationToken = CancellationToken.None;

        var kafkaProducerMock = new Mock<IProducer<long, string>>(MockBehavior.Strict);
        kafkaProducerMock
            .Setup(x => x.ProduceAsync(
                topic,
                It.Is<Message<long, string>>(e =>
                    e.Key == chatId &&
                    e.Value == JsonSerializer.Serialize(
                        new UpdateContext(
                            botId,
                            update,
                            dateTimeOffset,
                            isTest),
                        JsonBotAPI.Options)),
                cancellationToken))
            .ReturnsAsync(new DeliveryReport<long, string>())
            .Verifiable(Times.Once);

        var kafkaProducerFactoryMock = new Mock<IKafkaProducerFactory<long, string>>(MockBehavior.Strict);
        kafkaProducerFactoryMock
            .Setup(x => x.Create(topic))
            .Returns(kafkaProducerMock.Object)
            .Verifiable(Times.Once);


        await new UpdateKafkaProducer(kafkaProducerFactoryMock.Object)
            .ProduceBotAsync(
                botId,
                update,
                dateTimeOffset,
                isTest,
                cancellationToken);


        kafkaProducerMock.VerifyAll();
        kafkaProducerMock.VerifyNoOtherCalls();

        kafkaProducerFactoryMock.VerifyAll();
        kafkaProducerFactoryMock.VerifyNoOtherCalls();
    }
}