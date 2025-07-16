namespace Quizitor.Redis.Storage;

public interface IRedisStorage<TValue>
{
    Task<bool> UpsertAsync(
        string key,
        TValue value,
        CancellationToken cancellationToken,
        TimeSpan? expiry = null);

    Task<TValue?> ReadAsync(
        string key,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        string key,
        CancellationToken cancellationToken);
}