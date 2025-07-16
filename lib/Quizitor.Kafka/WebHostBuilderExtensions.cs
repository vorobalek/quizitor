using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Quizitor.Kafka;

public static class WebHostBuilderExtensions
{
    public static IWebHostBuilder AddKafka(this IWebHostBuilder builder, Action<KafkaOptions>? configureOptions = null)
    {
        return builder
            .ConfigureServices(services => services
                .AddSingleton(typeof(IKafkaProducerFactory<>), typeof(KafkaProducerFactory<>))
                .AddSingleton(typeof(IKafkaProducerFactory<,>), typeof(KafkaProducerFactory<,>))
                .AddOptions<KafkaOptions>()
                .Configure(options =>
                {
                    options.BootstrapServers = KafkaConfiguration.BootstrapServers;
                    options.DefaultNumPartitions = KafkaConfiguration.DefaultNumPartitions;
                    options.DefaultReplicationFactor = KafkaConfiguration.DefaultReplicationFactor;
                    configureOptions?.Invoke(options);
                }));
    }

    public static IServiceCollection AddKafkaConsumer<TConsumer>(this IServiceCollection services)
        where TConsumer : KafkaConsumerTask
    {
        return services
            .AddHostedService<TConsumer>();
    }
}