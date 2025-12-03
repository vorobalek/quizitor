using Quizitor.Common;

namespace Quizitor.Bots.Configuration;

public static class KafkaConfiguration
{
    public static readonly string ConsumerGroupId = "KAFKA_CONSUMER_GROUP_ID"
        .GetEnvironmentValueWithFallback("Quizitor.Bots");
}