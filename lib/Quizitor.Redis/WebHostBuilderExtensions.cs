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
    extension(IWebHostBuilder builder)
    {
        public IWebHostBuilder AddRedis()
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
    }

    extension(IServiceCollection services)
    {
        private IServiceCollection AddRedisStorage<TValue>()
        {
            return services.AddScoped<IRedisStorage<TValue>, RedisStorage<TValue>>();
        }
    }
}