using Quizitor.Redis.Contracts;

namespace Quizitor.Redis.Storage.Rating;

internal sealed class RatingFullFinalRedisStorage(
    IRedisStorage<RatingFullDto> redisStorage) :
    RatingRedisStorage<RatingFullDto>(redisStorage),
    IRatingFullFinalRedisStorage
{
    protected override string KeyTemplate => RatingFullDto.SessionRedisFinalKey;
}