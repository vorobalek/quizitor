using Quizitor.Redis.Contracts;

namespace Quizitor.Redis.Storage.Rating;

internal sealed class RatingFullStageRedisStorage(
    IRedisStorage<RatingFullDto> redisStorage) :
    RatingRedisStorage<RatingFullDto>(redisStorage),
    IRatingFullStageRedisStorage
{
    protected override string KeyTemplate => RatingFullDto.SessionRedisStageKey;
}