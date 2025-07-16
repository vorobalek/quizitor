using Quizitor.Redis.Contracts;

namespace Quizitor.Redis.Storage.Rating;

internal abstract class RatingRedisStorage<TValue>(
    IRedisStorage<TValue> redisStorage) : IRatingRedisStorage<TValue>
    where TValue : IRating
{
    protected abstract string KeyTemplate { get; }

    public Task<bool> UpsertAsync(
        TValue value,
        CancellationToken cancellationToken,
        TimeSpan? expiry = null)
    {
        return redisStorage
            .UpsertAsync(
                string
                    .Format(
                        KeyTemplate,
                        value.SessionId),
                value,
                cancellationToken,
                expiry ?? TimeSpan.FromDays(1));
    }

    public Task<TValue?> ReadAsync(
        int sessionId,
        CancellationToken cancellationToken)
    {
        return redisStorage
            .ReadAsync(
                string
                    .Format(
                        KeyTemplate,
                        sessionId),
                cancellationToken);
    }
}