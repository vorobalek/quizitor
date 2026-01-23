using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Quizitor.Kafka;

public delegate Task KafkaConsumerRunnerDelegate(CancellationToken cancellationToken);

public abstract partial class KafkaConsumerTask(
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
                await ExecuteOnceAsync(restartsCount++, stoppingToken);
            }
            catch (Exception exception)
            {
                LogBackgroundLoopFailedException(logger, GetType().Name, exception);
            }
        }
    }

    private async Task ExecuteOnceAsync(int restartsCount, CancellationToken stoppingToken)
    {
        LogTaskStartedWithRestartsCount(logger, GetType().Name, restartsCount);
        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        var cancellationToken = cancellationTokenSource.Token;
        var runnerDelegates = await GetConsumerRunners(cancellationToken);
        var runners = runnerDelegates.Select(runner => runner.Invoke(cancellationToken)).ToArray();
        await Task.WhenAny(runners);
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
        return stoppingToken => Task.Run(async () =>
        {
            await EnsureTopicExists(topic, numPartitionsValue, replicationFactorValue);

            var restartsCount = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                LogConsumerRunnerStartedWithRestartsCount(logger, topic, restartsCount++);
                using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                var cancellationToken = cancellationTokenSource.Token;
                await ConsumerProcess(topic, groupId, consumerTask, cancellationToken);
            }
        }, stoppingToken);
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
            LogTopicTopicHasBeenCreatedWithPartitionsAndReplicationFactor(logger, topic, numPartitions, replicationFactor);
        }
        catch (CreateTopicsException e)
        {
            if (e.Results.Any(r => r.Error.Code == ErrorCode.TopicAlreadyExists))
                LogTopicIsAlreadyCreatedByAnotherClient(logger, topic);
            else
                throw;
        }
    }

    private async Task ConsumerProcess<TKey, TMessage>(
        string topic,
        string groupId,
        Func<ConsumeResult<TKey, TMessage>, CancellationToken, Task> consumerTask,
        CancellationToken stoppingToken)
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
        LogTopicSubscribed(logger, topic);
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(stoppingToken);
                if (consumeResult is null)
                {
                    await Task.Delay(100, stoppingToken);
                    continue;
                }
                
                using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                var cancellationToken = cancellationTokenSource.Token;
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
        catch (Exception exception) when (exception is TaskCanceledException or OperationCanceledException && stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            LogConsumerRunnerFailed(logger, topic, exception);
        }
        finally
        {
            consumer.Close();
        }
    }

    [LoggerMessage(LogLevel.Error, "{name} background loop failed.")]
    static partial void LogBackgroundLoopFailedException(ILogger logger, string name, Exception exception);

    [LoggerMessage(LogLevel.Warning, "{name} started. Restarts count: {restartsCount}")]
    static partial void LogTaskStartedWithRestartsCount(ILogger logger, string name, int restartsCount);

    [LoggerMessage(LogLevel.Warning, "Topic '{topic}' has been created with {numPartitions} partition(s) and replication factor {replicationFactor}")]
    static partial void LogTopicTopicHasBeenCreatedWithPartitionsAndReplicationFactor(ILogger logger, string topic, int numPartitions, short replicationFactor);

    [LoggerMessage(LogLevel.Warning, "Topic '{topic}' is already created by another client")]
    static partial void LogTopicIsAlreadyCreatedByAnotherClient(ILogger logger, string topic);

    [LoggerMessage(LogLevel.Warning, "'{topic}' subscribed")]
    static partial void LogTopicSubscribed(ILogger logger, string topic);

    [LoggerMessage(LogLevel.Error, "{topic} consumer runner failed")]
    static partial void LogConsumerRunnerFailed(ILogger logger, string topic, Exception exception);
    
    [LoggerMessage(LogLevel.Warning, "{topic} consumer runner started. Restarts count: {restartsCount}")]
    static partial void LogConsumerRunnerStartedWithRestartsCount(ILogger logger, string topic, int restartsCount);
}