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

internal sealed class WebhookService(
    ITelegramBotClientFactory clientFactory,
    IOptions<AppOptions> options,
    IOptions<TelegramBotSecrets> secrets,
    IDbContextProvider dbContextProvider,
    ILogger<WebhookService> logger) : IWebhookService
{
    public async Task<TelegramUser> SetForBotAsync(Bot bot, CancellationToken cancellationToken)
    {
        var webhookAddress = $"{options.Value.Url}/bot/{bot.Id}";
        logger.LogWarning(
            "[{Id}] Setting webhook: {WebhookAddress}",
            bot.Id,
            webhookAddress);
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
                botCommands.Select(BotCommandExtensions.ToTelegramBotCommand),
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

        logger.LogWarning(
            "[{Id}:{Username}] Webhook set: {WebhookAddress}",
            bot.Id,
            botUser.Username,
            webhookAddress);

        return botUser;
    }

    public async Task<TelegramUser> SetDefaultAsync(CancellationToken cancellationToken)
    {
        var webhookAddress = $"{options.Value.Url}/bot";
        logger.LogWarning("[BO] Setting webhook: {WebhookAddress}", webhookAddress);
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
        logger.LogWarning("[BO:{Username}] Webhook set: {WebhookAddress}", botUser.Username, webhookAddress);

        return botUser;
    }

    public async Task<TelegramUser> DeleteForBotAsync(Bot bot, CancellationToken cancellationToken)
    {
        logger.LogWarning("[{Id}] Removing webhook", bot.Id);
        var client = clientFactory.CreateForBot(bot);
        var botUser = await client.GetMe(cancellationToken);
        await client
            .DeleteWebhook(
                bot.DropPendingUpdates,
                cancellationToken);
        logger.LogWarning("[{Id}:{Username}] Webhook removed", bot.Id, botUser.Username);

        return botUser;
    }

    public async Task<TelegramUser> DeleteDefaultAsync(CancellationToken cancellationToken)
    {
        logger.LogWarning("[BO] Removing webhook");
        var client = clientFactory.CreateDefault();
        var botUser = await client.GetMe(cancellationToken);
        await client
            .DeleteWebhook(
                true,
                cancellationToken);
        logger.LogWarning("[BO:{Username}] Webhook removed", botUser.Username);

        return botUser;
    }
}