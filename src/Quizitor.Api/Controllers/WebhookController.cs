using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Quizitor.Api.Services.Kafka.Updates;
using Quizitor.Api.Services.TelegramBot.ClientFactory;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Api.Controllers;

public sealed class WebhookController(
    IDbContextProvider dbContextProvider,
    ITelegramBotClientFactory telegramBotClientFactory,
    IUpdateKafkaProducer updateKafkaProducer,
    IGlobalCancellationTokenSource cancellationTokenSource) : ControllerBase
{
    private static readonly Histogram ApiWebhookBackOfficeHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}api_webhook_backoffice",
        "Histogram of backoffice update api webhook.",
        new HistogramConfiguration
        {
            Buckets =
            [
                .05, .1, .2, .3, .4, .5, .6, .7, .8, .9, 1, 2, 5, 10, 30, 60
            ],
            LabelNames =
            [
                "is_test"
            ]
        });

    private static readonly Histogram ApiWebhookBotHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}api_webhook_bot",
        "Histogram of bot's update api webhook.",
        new HistogramConfiguration
        {
            Buckets =
            [
                .05, .1, .2, .3, .4, .5, .6, .7, .8, .9, 1, 2, 5, 10, 30, 60
            ],
            LabelNames =
            [
                "bot_id",
                "is_test"
            ]
        });

    [HttpPost("/bot")]
    public async Task<IActionResult> Post([FromBody] Update update, [FromQuery(Name = "is_test")] bool isTest = false)
    {
        IActionResult result;
        using (ApiWebhookBackOfficeHistogram.WithLabels([isTest.ToString()]).NewTimer())
        {
            var cancellationToken = cancellationTokenSource.Token;
            result = await ProcessAsync(null, update, isTest, cancellationToken);
        }

        return result;
    }

    [HttpPost("/bot/{botId:int}")]
    public async Task<IActionResult> Post(int botId, [FromBody] Update update, [FromQuery(Name = "is_test")] bool isTest = false)
    {
        IActionResult result;
        using (ApiWebhookBotHistogram.WithLabels([
                   botId.ToString(),
                   isTest.ToString()
               ]).NewTimer())
        {
            var cancellationToken = cancellationTokenSource.Token;
            result = await ProcessAsync(botId, update, isTest, cancellationToken);
        }

        return result;
    }

    private async Task<IActionResult> ProcessAsync(
        int? botId,
        Update update,
        bool isTest,
        CancellationToken cancellationToken)
    {
        var initiatedAt = await dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);
        if (botId.HasValue)
        {
            if (await dbContextProvider
                    .Bots
                    .GetByIdOrDefaultAsync(
                        botId.Value,
                        cancellationToken) is not { } bot)
                return NotFound();

            if (bot.IsActive)
                await SendPreActionAsync(bot, update, initiatedAt, isTest, cancellationToken);

            if (bot.IsActive || !bot.DropPendingUpdates)
                await updateKafkaProducer.ProduceBotAsync(
                    bot.Id,
                    update,
                    bot.IsActive ? initiatedAt : null,
                    isTest,
                    cancellationToken);
        }
        else
        {
            await SendPreActionAsync(null, update, initiatedAt, isTest, cancellationToken);
            await updateKafkaProducer.ProduceAsync(
                update,
                initiatedAt,
                isTest,
                cancellationToken);
        }

        return Ok();
    }

    private Task SendPreActionAsync(
        Bot? bot,
        Update update,
        DateTimeOffset initiatedAt,
        bool isTest,
        CancellationToken cancellationToken)
    {
        return update.Type switch
        {
            UpdateType.Message when update.Message is { } message =>
                (bot is not null
                    ? telegramBotClientFactory.CreateForBot(bot)
                    : telegramBotClientFactory.CreateDefault())
                .SendChatAction(
                    new UpdateContext(bot?.Id, update, initiatedAt, isTest),
                    message.Chat.Id,
                    ChatAction.Typing,
                    cancellationToken: cancellationToken),
            _ => Task.CompletedTask
        };
    }
}