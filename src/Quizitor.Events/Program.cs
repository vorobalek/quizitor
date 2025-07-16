using Prometheus;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Events.Configuration;
using Quizitor.Events.Services;
using Quizitor.Events.Services.Kafka;
using Quizitor.Kafka;
using Quizitor.Logging;
using Quizitor.Redis;

Metrics.SuppressDefaultMetrics();
await Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder => builder
        .AddGlobalCancellationToken()
        .AddDatabase(AppConfiguration.DbConnectionString)
        .AddLogging(SentryConfiguration.MinimumEventLevel, SentryConfiguration.Dsn)
        .AddKafka()
        .AddRedis()
        .ConfigureServices(services => services
            .AddHostedService<CalculateRatingStageProcessing>()
            .AddHostedService<CalculateRatingFinalProcessing>()
            .AddHostedService<QuestionTimingNotifyEventProcessing>()
            .AddHostedService<QuestionTimingStopEventProcessing>()
            .AddScoped<IQuestionTimingNotifyKafkaProducer, QuestionTimingNotifyKafkaProducer>()
            .AddScoped<IQuestionTimingStopKafkaProducer, QuestionTimingStopKafkaProducer>()
            .AddHealthChecks())
        .Configure(app => app
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
            })
            .UseHealthChecks("/health"))
        .UseUrls($"http://+:{AppConfiguration.Port}"))
    .Build()
    .RunAsync();