using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Quizitor.Common;
using Quizitor.Redis.Configuration;
using Quizitor.Redis.Contracts;
using Quizitor.Redis.Serializers;
using Quizitor.Redis.Storage;
using Quizitor.Redis.Storage.Rating;
using StackExchange.Redis;

namespace Quizitor.Redis;

public static class WebHostBuilderExtensions
{
    public static IWebHostBuilder AddRedis(this IWebHostBuilder builder)
    {
        return builder.ConfigureServices(services => services
            .AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(RedisConfiguration.RedisConnectionString))
            .AddJsonSerializer<RatingShortDto, RedisDefaultSerializer<RatingShortDto>>()
            .AddRedisStorage<RatingShortDto>()
            .AddScoped<IRatingShortStageRedisStorage, RatingShortStageRedisStorage>()
            .AddScoped<IRatingShortFinalRedisStorage, RatingShortFinalRedisStorage>()
            .AddJsonSerializer<RatingFullDto, RedisDefaultSerializer<RatingFullDto>>()
            .AddRedisStorage<RatingFullDto>()
            .AddScoped<IRatingFullStageRedisStorage, RatingFullStageRedisStorage>()
            .AddScoped<IRatingFullFinalRedisStorage, RatingFullFinalRedisStorage>());
    }

    private static IServiceCollection AddRedisStorage<TValue>(this IServiceCollection services)
    {
        return services.AddScoped<IRedisStorage<TValue>, RedisStorage<TValue>>();
    }
}