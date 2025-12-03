using LPlus;
using Microsoft.Extensions.Options;
using Quizitor.Api.Options;
using Quizitor.Api.Services.TelegramBot.ClientFactory;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Data.Extensions;
using Telegram.Bot.Types.Enums;
using BotCommand = Telegram.Bot.Types.BotCommand;

namespace Quizitor.Api.Services.ConfigureWebhook;

internal sealed partial class WebhookService(
    ITelegramBotClientFactory clientFactory,
    IOptions<AppOptions> options,
    IOptions<TelegramBotSecrets> secrets,
    IDbContextProvider dbContextProvider,
    ILogger<WebhookService> logger) : IWebhookService
{
    public async Task<TelegramUser> SetForBotAsync(Bot bot, CancellationToken cancellationToken)
    {
        var webhookAddress = $"{options.Value.Url}/bot/{bot.Id}";
        LogSettingWebhook(logger, bot.Id, webhookAddress);
        var client = clientFactory.CreateForBot(bot);
        await client
            .SetWebhook(
                webhookAddress,
                secretToken: secrets.Value.Token,
                allowedUpdates:
                [
                    UpdateType.Message,
                    UpdateType.CallbackQuery
                ],
                dropPendingUpdates: bot.DropPendingUpdates,
                cancellationToken: cancellationToken);
        await client
            .DeleteMyCommands(
                cancellationToken: cancellationToken);
        var botCommands = await dbContextProvider
            .BotCommands
            .GetByTypeAsync(
                bot.Type,
                cancellationToken);
        await client
            .SetMyCommands(
                botCommands.Select(x => x.TelegramBotCommand),
                cancellationToken: cancellationToken);
        var botUser = await client.GetMe(cancellationToken);
        if (bot.Username is null)
        {
            bot.Username = botUser.Username ?? throw new InvalidOperationException("Bot's username is null");
            await dbContextProvider
                .Bots
                .UpdateAsync(
                    bot,
                    cancellationToken);
        }

        LogWebhookSet(logger, bot.Id, botUser.Username, webhookAddress);

        return botUser;
    }

    public async Task<TelegramUser> SetDefaultAsync(CancellationToken cancellationToken)
    {
        var webhookAddress = $"{options.Value.Url}/bot";
        LogBoSettingWebhook(logger, webhookAddress);
        var client = clientFactory.CreateDefault();
        await client
            .SetWebhook(
                webhookAddress,
                secretToken: secrets.Value.Token,
                allowedUpdates:
                [
                    UpdateType.Message,
                    UpdateType.CallbackQuery
                ],
                dropPendingUpdates: true,
                cancellationToken: cancellationToken);
        await client
            .SetMyCommands(
                [
                    new BotCommand
                    {
                        Command = "/start",
                        Description = TR.L + "_BACKOFFICE_START_COMMAND"
                    }
                ],
                cancellationToken: cancellationToken);
        var botUser = await client.GetMe(cancellationToken);
        LogBoWebhookSet(logger, botUser.Username, webhookAddress);

        return botUser;
    }

    public async Task<TelegramUser> DeleteForBotAsync(Bot bot, CancellationToken cancellationToken)
    {
        LogRemovingWebhook(logger, bot.Id);
        var client = clientFactory.CreateForBot(bot);
        var botUser = await client.GetMe(cancellationToken);
        await client
            .DeleteWebhook(
                bot.DropPendingUpdates,
                cancellationToken);
        LogWebhookRemoved(logger, bot.Id, botUser.Username);

        return botUser;
    }

    public async Task<TelegramUser> DeleteDefaultAsync(CancellationToken cancellationToken)
    {
        LogBoRemovingWebhook(logger);
        var client = clientFactory.CreateDefault();
        var botUser = await client.GetMe(cancellationToken);
        await client
            .DeleteWebhook(
                true,
                cancellationToken);
        LogBoWebhookRemoved(logger, botUser.Username);

        return botUser;
    }

    [LoggerMessage(LogLevel.Warning, "[{id}] Setting webhook: {webhookAddress}")]
    static partial void LogSettingWebhook(ILogger<WebhookService> logger, int id, string webhookAddress);

    [LoggerMessage(LogLevel.Warning, "[{id}:{username}] Webhook set: {webhookAddress}")]
    static partial void LogWebhookSet(ILogger<WebhookService> logger, int id, string? username, string webhookAddress);

    [LoggerMessage(LogLevel.Warning, "[BO] Setting webhook: {webhookAddress}")]
    static partial void LogBoSettingWebhook(ILogger<WebhookService> logger, string webhookAddress);

    [LoggerMessage(LogLevel.Warning, "[BO:{username}] Webhook set: {webhookAddress}")]
    static partial void LogBoWebhookSet(ILogger<WebhookService> logger, string? username, string webhookAddress);

    [LoggerMessage(LogLevel.Warning, "[{id}] Removing webhook")]
    static partial void LogRemovingWebhook(ILogger<WebhookService> logger, int id);

    [LoggerMessage(LogLevel.Warning, "[{id}:{username}] Webhook removed")]
    static partial void LogWebhookRemoved(ILogger<WebhookService> logger, int id, string? username);

    [LoggerMessage(LogLevel.Warning, "[BO] Removing webhook")]
    static partial void LogBoRemovingWebhook(ILogger<WebhookService> logger);

    [LoggerMessage(LogLevel.Warning, "[BO:{username}] Webhook removed")]
    static partial void LogBoWebhookRemoved(ILogger<WebhookService> logger, string? username);
}