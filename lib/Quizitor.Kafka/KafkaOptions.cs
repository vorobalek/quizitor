using Confluent.Kafka;

namespace Quizitor.Kafka;

public class KafkaOptions
{
    public string BootstrapServers { get; set; } = null!;
    public string ConsumerGroupId { get; set; } = null!;
    public int DefaultNumPartitions { get; set; } = 1;
    public short DefaultReplicationFactor { get; set; } = 1;
    public CompressionType CompressionType { get; set; } = CompressionType.Gzip;
}