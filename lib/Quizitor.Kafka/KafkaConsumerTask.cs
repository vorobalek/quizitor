using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Quizitor.Kafka;

public delegate Task KafkaConsumerRunnerDelegate(CancellationToken cancellationToken);

public abstract class KafkaConsumerTask(
    IOptions<KafkaOptions> options,
    ILogger logger) : BackgroundService
{
    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var restartsCount = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteOnceAsync(restartsCount, stoppingToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "{Name} background loop failed", GetType().Name);
            }

            restartsCount++;
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task ExecuteOnceAsync(int restartsCount, CancellationToken stoppingToken)
    {
        logger.LogWarning("{Name} started. Restarts count: {RestartsCount}", GetType().Name, restartsCount);
        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        var token = cancellationTokenSource.Token;
        var runners = await GetConsumerRunners(token);
        await Task.WhenAny(runners.Select(runner => runner.Invoke(token)));
        await cancellationTokenSource.CancelAsync();
    }

    protected abstract Task<KafkaConsumerRunnerDelegate[]> GetConsumerRunners(CancellationToken stoppingToken);

    protected KafkaConsumerRunnerDelegate CreateConsumerRunner<TKey, TMessage>(
        string topic,
        string groupId,
        Func<ConsumeResult<TKey, TMessage>, CancellationToken, Task> consumerTask)
    {
        var numPartitionsValue = options.Value.DefaultNumPartitions;
        var replicationFactorValue = options.Value.DefaultReplicationFactor;
        return cancellationToken => Task.Run(async () =>
        {
            await EnsureTopicExists(topic, numPartitionsValue, replicationFactorValue);
            await ConsumerProcess(topic, groupId, consumerTask, cancellationToken);
        }, cancellationToken);
    }

    private async Task EnsureTopicExists(
        string topic,
        int numPartitions,
        short replicationFactor)
    {
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = options.Value.BootstrapServers
        };
        using var adminClient = new AdminClientBuilder(adminConfig).Build();
        var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
        var existingTopic = metadata.Topics.FirstOrDefault(t => t.Topic == topic);
        if (existingTopic != null && existingTopic.Error.Code != ErrorCode.UnknownTopicOrPart) return;
        try
        {
            await adminClient.CreateTopicsAsync([
                new TopicSpecification
                {
                    Name = topic,
                    NumPartitions = numPartitions,
                    ReplicationFactor = replicationFactor
                }
            ]);

            await Task.Delay(TimeSpan.FromSeconds(1));

            logger.LogWarning(
                "Topic '{Topic}' has been created with {NumPartitions} partition(s) and replication factor {ReplicationFactor}",
                topic, numPartitions, replicationFactor);
        }
        catch (CreateTopicsException e)
        {
            if (e.Results.Any(r => r.Error.Code == ErrorCode.TopicAlreadyExists))
                logger.LogWarning("Topic '{Topic}' is already created by another client", topic);
            else
                throw;
        }
    }

    private async Task ConsumerProcess<TKey, TMessage>(
        string topic,
        string groupId,
        Func<ConsumeResult<TKey, TMessage>, CancellationToken, Task> consumerTask,
        CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false
        };

        using var consumer = new ConsumerBuilder<TKey, TMessage>(config).Build();
        consumer.Subscribe(topic);
        logger.LogWarning("Subscribed to '{Topic}'", topic);
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(cancellationToken);
                if (consumeResult is null)
                {
                    await Task.Delay(100, cancellationToken);
                    continue;
                }

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await consumerTask(consumeResult, cancellationToken);
                        break;
                    }
                    catch (RetryLaterExtension retryLaterExtension)
                    {
                        await Task.Delay(retryLaterExtension.Delay, cancellationToken);
                    }
                }

                consumer.Commit(consumeResult);
            }
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception e)
        {
            logger.LogError(e, "{Name} consumer task failed", GetType().Name);
        }
        finally
        {
            consumer.Close();
        }
    }
}