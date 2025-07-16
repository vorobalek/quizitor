using Prometheus;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Extensions;

namespace Quizitor.Api.Services.ConfigureWebhook;

internal sealed class ConfigureWebhookHostedService(
    IServiceScopeFactory serviceScopeFactory)
    : IHostedService
{
    private static readonly Gauge BackOfficeInfoGauge = Metrics.CreateGauge(
        $"{Constants.MetricsPrefix}backoffice_info",
        "BackOffice Info",
        new GaugeConfiguration
        {
            LabelNames =
            [
                "username"
            ]
        });

    private static readonly Gauge BotInfoGauge = Metrics.CreateGauge(
        $"{Constants.MetricsPrefix}bot_info",
        "Bot Info",
        new GaugeConfiguration
        {
            LabelNames =
            [
                "bot_id",
                "username"
            ]
        });

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return serviceScopeFactory.ExecuteUnitOfWorkWithRetryAsync(async asyncScope =>
            {
                var webhookService = asyncScope.ServiceProvider.GetRequiredService<IWebhookService>();
                var serviceBotUser = await webhookService.SetDefaultAsync(cancellationToken);

                BackOfficeInfoGauge
                    .WithLabels(
                    [
                        serviceBotUser.Username!
                    ])
                    .Set(1);

                var dbContextProvider = asyncScope.ServiceProvider.GetRequiredService<IDbContextProvider>();
                var bots = await dbContextProvider
                    .Bots
                    .GetAllAsync(cancellationToken);
                foreach (var bot in bots)
                {
                    var botUser = await webhookService.SetForBotAsync(bot, cancellationToken);

                    BotInfoGauge
                        .WithLabels(
                        [
                            bot.Id.ToString(),
                            botUser.Username ?? throw new InvalidOperationException("Bot's username is null")
                        ])
                        .Set(1);
                }
            },
            cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return serviceScopeFactory.ExecuteUnitOfWorkWithRetryAsync(async asyncScope =>
            {
                var webhookService = asyncScope.ServiceProvider.GetRequiredService<IWebhookService>();
                var dbContextProvider = asyncScope.ServiceProvider.GetRequiredService<IDbContextProvider>();
                var bots = await dbContextProvider
                    .Bots
                    .GetAllAsync(cancellationToken);
                foreach (var bot in bots)
                {
                    var botUser = await webhookService.DeleteForBotAsync(bot, cancellationToken);

                    BotInfoGauge
                        .WithLabels(
                        [
                            bot.Id.ToString(),
                            botUser.Username ?? throw new InvalidOperationException("Bot's username is null")
                        ])
                        .Set(0);
                }

                var serviceBotUser = await webhookService.DeleteDefaultAsync(cancellationToken);

                BackOfficeInfoGauge
                    .WithLabels(
                    [
                        serviceBotUser.Username!
                    ])
                    .Set(1);
            },
            cancellationToken);
    }
}