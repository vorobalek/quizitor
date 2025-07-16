using Telegram.Bot.Types;

namespace Quizitor.Kafka.Contracts;

// ReSharper disable once ClassNeverInstantiated.Global
public record UpdateContext(
    int? BotId,
    Update Update,
    DateTimeOffset? InitiatedAt,
    bool IsTest);