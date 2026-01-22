using Prometheus;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Kafka;
using Quizitor.Logging;
using Quizitor.Sender.Configuration;
using Quizitor.Sender.Services;
using Quizitor.Sender.Services.Infrastructure;

Metrics.SuppressDefaultMetrics();
await Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder => builder
        .AddGlobalCancellationToken()
        .AddDatabase(AppConfiguration.DbConnectionString)
        .AddLogging(SentryConfiguration.MinimumEventLevel, SentryConfiguration.Dsn)
        .AddKafka(options => options.ConsumerGroupId = KafkaConfiguration.ConsumerGroupId)
        .ConfigureServices(services => services
            .AddSingleton<IBotListCache, BotListCache>()
            .AddKafkaConsumer<AnswerCallbackQueryKafkaConsumerTask>()
            .AddKafkaConsumer<DeleteMessageKafkaConsumerTask>()
            .AddKafkaConsumer<EditMessageKafkaConsumerTask>()
            .AddKafkaConsumer<SendChatActionKafkaConsumerTask>()
            .AddKafkaConsumer<SendMessageKafkaConsumerTask>()
            .AddKafkaConsumer<SendPhotoKafkaConsumerTask>()
            .AddHttpClient()
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