using Telegram.Bot.Types;

namespace Quizitor.Api.Services.Kafka.Updates;

public interface IUpdateKafkaProducer
{
    Task ProduceAsync(
        Update update,
        DateTimeOffset? initiatedAt,
        bool isTest,
        CancellationToken cancellationToken);

    Task ProduceBotAsync(
        int botId,
        Update update,
        DateTimeOffset? initiatedAt,
        bool isTest,
        CancellationToken cancellationToken);
}