global using TelegramUser = Telegram.Bot.Types.User;
using System.Runtime.CompilerServices;
using Prometheus;
using Quizitor.Api.Configuration;
using Quizitor.Api.Middleware;
using Quizitor.Api.Options;
using Quizitor.Api.Services.ConfigureWebhook;
using Quizitor.Api.Services.ExceptionHandler;
using Quizitor.Api.Services.HttpContext.RequestBodyReader;
using Quizitor.Api.Services.HttpContext.RequestCollector;
using Quizitor.Api.Services.Kafka.SendChatAction;
using Quizitor.Api.Services.Kafka.Updates;
using Quizitor.Api.Services.TelegramBot.ClientFactory;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Kafka;
using Quizitor.Localization;
using Quizitor.Logging;
using Telegram.Bot.AspNetCore;

[assembly: InternalsVisibleTo("Quizitor.Api.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

Metrics.SuppressDefaultMetrics();
await Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder => builder
        .AddGlobalCancellationToken()
        .AddLocalization(AppConfiguration.Locale)
        .AddDatabase(AppConfiguration.DbConnectionString)
        .AddLogging(SentryConfiguration.MinimumEventLevel, SentryConfiguration.Dsn)
        .AddKafka()
        .ConfigureServices(services =>
        {
            services
                .AddHostedService<ConfigureWebhookHostedService>()
                .AddSingleton<IExceptionHandlerService, ExceptionHandlerService>()
                .AddSingleton<IHttpContextRequestBodyReader, HttpContextRequestBodyReader>()
                .AddSingleton<IHttpContextRequestCollector, HttpContextRequestCollector>()
                .AddScoped<ExceptionHandlerMiddleware>()
                .AddScoped<SecretTokenValidatorMiddleware>()
                .AddScoped<IUpdateKafkaProducer, UpdateKafkaProducer>()
                .AddScoped<ISendChatActionKafkaProducer, SendChatActionKafkaProducer>()
                .AddScoped<ITelegramBotClientFactory, TelegramBotClientFactory>()
                .AddScoped<IWebhookService, WebhookService>()
                .AddHttpClient()
                .AddHttpContextAccessor()
                .ConfigureTelegramBotMvc();
            services
                .AddOptions<AppOptions>()
                .Configure(x =>
                {
                    x.Url = $"https://{"DOMAIN".RequiredEnvironmentValue}";
                });
            services
                .AddOptions<TelegramBotOptions>()
                .Configure(x =>
                {
                    x.Token = "TELEGRAM_BOT_TOKEN".RequiredEnvironmentValue;
                });
            services
                .AddOptions<TelegramBotSecrets>()
                .Configure(x =>
                {
                    x.HeaderName = "X-Telegram-Bot-Api-Secret-Token";
                    x.Token = "TELEGRAM_WEBHOOK_SECRET".RequiredEnvironmentValue;
                });

            services.AddControllers();
            services.AddHealthChecks();
        })
        .Configure(app =>
        {
            var pathBase = !string.IsNullOrWhiteSpace(AppConfiguration.PathBase)
                ? AppConfiguration.PathBase
                : string.Empty;
            app
                .UsePathBase(pathBase)
                .UseMiddleware<ExceptionHandlerMiddleware>()
                .UseWhen(
                    context => context.Request.Path.StartsWithSegments("/bot"),
                    a => a.UseMiddleware<SecretTokenValidatorMiddleware>())
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapMetrics();
                })
                .UseHealthChecks("/health");
        })
        .UseUrls($"http://+:{AppConfiguration.Port}"))
    .Build()
    .RunAsync();