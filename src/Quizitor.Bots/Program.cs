global using TelegramUser = Telegram.Bot.Types.User;
global using User = Quizitor.Data.Entities.User;
using Prometheus;
using Quizitor.Bots.Configuration;
using Quizitor.Bots.Extensions;
using Quizitor.Bots.Services.Crypto;
using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.Kafka.Consumers;
using Quizitor.Bots.Services.Kafka.Producers.AnswerCallbackQuery;
using Quizitor.Bots.Services.Kafka.Producers.DeleteMessage;
using Quizitor.Bots.Services.Kafka.Producers.EditMessage;
using Quizitor.Bots.Services.Kafka.Producers.SendChatAction;
using Quizitor.Bots.Services.Kafka.Producers.SendMessage;
using Quizitor.Bots.Services.Kafka.Producers.SendPhoto;
using Quizitor.Bots.Services.Qr;
using Quizitor.Bots.Services.QuestionRules;
using Quizitor.Bots.Services.TelegramBot.ClientFactory;
using Quizitor.Bots.Services.Updates;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Kafka;
using Quizitor.Localization;
using Quizitor.Logging;
using Quizitor.Redis;

Metrics.SuppressDefaultMetrics();
await Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder => builder
        .AddGlobalCancellationToken()
        .AddLocalization(AppConfiguration.Locale)
        .AddDatabase(AppConfiguration.DbConnectionString)
        .AddLogging(SentryConfiguration.MinimumEventLevel, SentryConfiguration.Dsn)
        .AddKafka(options => options.ConsumerGroupId = KafkaConfiguration.ConsumerGroupId)
        .AddRedis()
        .ConfigureServices(services => services
            .AddKafkaConsumer<UpdatesInboundKafkaConsumerTask>()
            .AddKafkaConsumer<TimingNotifyKafkaConsumerTask>()
            .AddKafkaConsumer<TimingStopKafkaConsumerTask>()
            .AddScoped<ISendMessageKafkaProducer, SendMessageKafkaProducer>()
            .AddScoped<IEditMessageKafkaProducer, EditMessageKafkaProducer>()
            .AddScoped<IAnswerCallbackQueryKafkaProducer, AnswerCallbackQueryKafkaProducer>()
            .AddScoped<IDeleteMessageKafkaProducer, DeleteMessageKafkaProducer>()
            .AddScoped<ISendChatActionKafkaProducer, SendChatActionKafkaProducer>()
            .AddScoped<ISendPhotoKafkaProducer, SendPhotoKafkaProducer>()
            .AddScoped<IIdentityService, IdentityService>()
            .AddScoped<ITelegramBotClientFactory, TelegramBotClientFactory>()
            .AddScoped<IUpdateService, UpdateService>()
            .AddBehaviors()
            .AddScoped<IQuestionRuleApplier, AnyAnswerQuestionRuleApplier>()
            .AddScoped<IQuestionRuleApplier, FirstAcceptedAnswerQuestionRuleApplier>()
            .AddSingleton<IQrService, QrService>()
            .AddSingleton<IEncryptionService, EncryptionService>()
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