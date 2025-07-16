using Quizitor.Redis.Contracts;

namespace Quizitor.Redis.Storage.Rating;

internal sealed class RatingShortStageRedisStorage(
    IRedisStorage<RatingShortDto> redisStorage) :
    RatingRedisStorage<RatingShortDto>(redisStorage),
    IRatingShortStageRedisStorage
{
    protected override string KeyTemplate => RatingShortDto.SessionRedisStageKey;
}