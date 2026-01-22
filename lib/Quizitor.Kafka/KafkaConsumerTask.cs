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
                await ExecuteOnceAsync(restartsCount, stoppingToken);
            }
            catch (Exception exception)
            {
                LogBackgroundLoopFailedException(logger, GetType().Name, exception);
            }

            restartsCount++;
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task ExecuteOnceAsync(int restartsCount, CancellationToken stoppingToken)
    {
        LogTaskStartedWithRestartsCount(logger, GetType().Name, restartsCount);
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
        Func<ConsumeResult<TKey, TMessage>, CancellationToken, Task> consumerTask,
        bool ensureTopicExists = true)
    {
        var numPartitionsValue = options.Value.DefaultNumPartitions;
        var replicationFactorValue = options.Value.DefaultReplicationFactor;
        return cancellationToken => Task.Run(async () =>
        {
            if (ensureTopicExists)
            {
                await EnsureTopicExists(topic, numPartitionsValue, replicationFactorValue, cancellationToken);
            }

            await ConsumerProcess(topic, groupId, consumerTask, cancellationToken);
        }, cancellationToken);
    }

    protected async Task EnsureTopicsExist(
        IEnumerable<string> topics,
        int numPartitions,
        short replicationFactor,
        CancellationToken cancellationToken)
    {
        var topicList = topics
            .Where(topic => !string.IsNullOrWhiteSpace(topic))
            .Distinct()
            .ToArray();
        if (topicList.Length == 0)
        {
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = options.Value.BootstrapServers
        };
        using var adminClient = new AdminClientBuilder(adminConfig).Build();
        var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
        var existingTopics = metadata
            .Topics
            .Select(topic => topic.Topic)
            .ToHashSet(StringComparer.Ordinal);
        TopicSpecification[] topicsToCreate =
        [
            .. topicList
                .Where(topic => !existingTopics.Contains(topic))
                .Select(topic => new TopicSpecification
                {
                    Name = topic,
                    NumPartitions = numPartitions,
                    ReplicationFactor = replicationFactor
                })
        ];
        if (topicsToCreate.Length == 0)
        {
            return;
        }

        try
        {
            await adminClient.CreateTopicsAsync(topicsToCreate);

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            foreach (var topic in topicsToCreate.Select(result => result.Name))
            {
                LogTopicTopicHasBeenCreatedWithPartitionsAndReplicationFactor(logger, topic, numPartitions, replicationFactor);
            }
        }
        catch (CreateTopicsException e)
        {
            if (e.Results.Any(result =>
                    result.Error.Code != ErrorCode.TopicAlreadyExists &&
                    result.Error.Code != ErrorCode.NoError))
            {
                throw;
            }

            foreach (var result in e.Results)
            {
                if (result.Error.Code == ErrorCode.TopicAlreadyExists)
                {
                    LogTopicIsAlreadyCreatedByAnotherClient(logger, result.Topic);
                }
            }

            var createdTopics = e.Results
                .Where(result => result.Error.Code == ErrorCode.NoError)
                .Select(result => result.Topic)
                .ToArray();
            if (createdTopics.Length > 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                foreach (var topic in createdTopics)
                {
                    LogTopicTopicHasBeenCreatedWithPartitionsAndReplicationFactor(logger, topic, numPartitions, replicationFactor);
                }
            }
        }
    }

    private async Task EnsureTopicExists(
        string topic,
        int numPartitions,
        short replicationFactor,
        CancellationToken cancellationToken)
    {
        await EnsureTopicsExist([topic], numPartitions, replicationFactor, cancellationToken);
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
        LogSubscribedToTopic(logger, topic);
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
        catch (Exception exception)
        {
            LogConsumerTaskFailed(logger, GetType().Name, exception);
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

    [LoggerMessage(LogLevel.Warning, "Subscribed to '{topic}'")]
    static partial void LogSubscribedToTopic(ILogger logger, string topic);

    [LoggerMessage(LogLevel.Error, "{name} consumer task failed")]
    static partial void LogConsumerTaskFailed(ILogger logger, string name, Exception exception);
}