using Quizitor.Redis.Contracts;

namespace Quizitor.Redis.Storage.Rating;

public interface IRatingRedisStorage<TValue>
    where TValue : IRating
{
    Task<bool> UpsertAsync(
        TValue value,
        CancellationToken cancellationToken,
        TimeSpan? expiry = null);

    Task<TValue?> ReadAsync(
        int sessionId,
        CancellationToken cancellationToken);
}