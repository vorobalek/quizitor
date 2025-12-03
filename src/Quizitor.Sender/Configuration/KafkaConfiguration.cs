using Quizitor.Common;

namespace Quizitor.Sender.Configuration;

public static class KafkaConfiguration
{
    public static readonly string ConsumerGroupId = "KAFKA_CONSUMER_GROUP_ID"
        .GetEnvironmentValueWithFallback("Quizitor.Sender");
}