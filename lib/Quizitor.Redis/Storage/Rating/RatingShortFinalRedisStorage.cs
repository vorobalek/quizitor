using Quizitor.Redis.Contracts;

namespace Quizitor.Redis.Storage.Rating;

internal sealed class RatingShortFinalRedisStorage(
    IRedisStorage<RatingShortDto> redisStorage) :
    RatingRedisStorage<RatingShortDto>(redisStorage),
    IRatingShortFinalRedisStorage
{
    protected override string KeyTemplate => RatingShortDto.SessionRedisFinalKey;
}