using Quizitor.Common;

namespace Quizitor.Kafka;

internal static class KafkaConfiguration
{
    private const int DefaultNumPartitionsInt32 = 1;

    private const short DefaultReplicationFactorInt16 = 1;

    public static readonly string BootstrapServers = "KAFKA_BOOTSTRAP_SERVERS"
        .RequiredEnvironmentValue;

    public static readonly int DefaultNumPartitions = "KAFKA_DEFAULT_NUM_PARTITIONS"
        .GetEnvironmentValueWithFallback(DefaultNumPartitionsInt32);

    public static readonly short DefaultReplicationFactor = "KAFKA_DEFAULT_REPLICATION_FACTOR"
        .GetEnvironmentValueWithFallback(DefaultReplicationFactorInt16);
}