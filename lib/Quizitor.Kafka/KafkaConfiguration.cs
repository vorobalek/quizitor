using Quizitor.Common;

namespace Quizitor.Kafka;

internal static class KafkaConfiguration
{
    private const int DefaultNumPartitionsInt32 = 1;

    private const short DefaultReplicationFactorInt16 = 1;

    public static readonly string BootstrapServers = "KAFKA_BOOTSTRAP_SERVERS"
        .GetEnvironmentVariableOrThrowIfNullOrWhiteSpace();

    public static readonly int DefaultNumPartitions = "KAFKA_DEFAULT_NUM_PARTITIONS"
        .GetEnvironmentVariableWithFallbackValue(DefaultNumPartitionsInt32);

    public static readonly short DefaultReplicationFactor = "KAFKA_DEFAULT_REPLICATION_FACTOR"
        .GetEnvironmentVariableWithFallbackValue(DefaultReplicationFactorInt16);
}