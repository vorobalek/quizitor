using Quizitor.Common;
using Quizitor.Redis.Configuration;
using StackExchange.Redis;

namespace Quizitor.Redis.Storage;

// ReSharper disable once ClassNeverInstantiated.Global
internal class RedisStorage<TValue> : IRedisStorage<TValue>
{
    private readonly IDatabase _database;
    private readonly IJsonSerializer<TValue> _serializer;

    public RedisStorage(
        IConnectionMultiplexer connectionMultiplexer,
        IJsonSerializer<TValue> serializer)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        _database = connectionMultiplexer.GetDatabase(DatabaseId);
        _serializer = serializer;
    }

    protected virtual int DatabaseId => 0;

    protected virtual string? KeyPrefix => RedisConfiguration.RedisKeyPrefix;

    public async Task<bool> UpsertAsync(
        string key,
        TValue value,
        CancellationToken cancellationToken,
        TimeSpan? expiry = null)
    {
        return await UpsertInternalAsync(key, value, cancellationToken, expiry);
    }

    public async Task<TValue?> ReadAsync(
        string publicKey,
        CancellationToken cancellationToken)
    {
        return await ReadInternalAsync(publicKey, cancellationToken);
    }

    public async Task<bool> DeleteAsync(
        string key,
        CancellationToken cancellationToken)
    {
        return await DeleteInternalAsync(key, cancellationToken);
    }

    protected virtual async Task<bool> UpsertInternalAsync(
        string key,
        TValue value,
        CancellationToken cancellationToken,
        TimeSpan? expiry = null)
    {
        return await _database.StringSetAsync(
            GetKey(key),
            _serializer.Serialize(value),
            expiry);
    }

    protected virtual async Task<TValue?> ReadInternalAsync(
        string key,
        CancellationToken cancellationToken)
    {
        string? redisValue = await _database.StringGetAsync(GetKey(key));
        return redisValue != null
            ? _serializer.Deserialize(redisValue)
            : default;
    }

    protected virtual async Task<bool> DeleteInternalAsync(
        string key,
        CancellationToken cancellationToken)
    {
        return await _database.KeyDeleteAsync(GetKey(key));
    }

    private string GetKey(string key)
    {
        return $"{(string.IsNullOrWhiteSpace(KeyPrefix) ? string.Empty : $"{KeyPrefix}:")}{key}";
    }
}