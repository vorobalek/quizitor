using Quizitor.Api.Services.Kafka.SendChatAction;
using Quizitor.Kafka.Contracts;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Api.Services.TelegramBot.ClientWrapper;

internal sealed class TelegramBotClientWrapper(
    int? botId,
    ITelegramBotClient botClient,
    ISendChatActionKafkaProducer sendChatActionKafkaProducer)
    : ITelegramBotClientWrapper
{
    public async Task SendChatAction(
        UpdateContext updateContext,
        ChatId chatId,
        ChatAction action,
        int? messageThreadId = null,
        string? businessConnectionId = null,
        CancellationToken cancellationToken = default)
    {
        var message = new SendChatActionRequest
        {
            ChatId = chatId,
            Action = action,
            MessageThreadId = messageThreadId,
            BusinessConnectionId = businessConnectionId
        };

        await (botId is null
            ? sendChatActionKafkaProducer.ProduceAsync(
                message,
                updateContext,
                cancellationToken)
            : sendChatActionKafkaProducer.ProduceBotAsync(
                botId.Value,
                message,
                updateContext,
                cancellationToken));
    }

    public Task SetWebhook(
        string url,
        InputFileStream? certificate = null,
        string? ipAddress = null,
        int? maxConnections = null,
        IEnumerable<UpdateType>? allowedUpdates = null,
        bool dropPendingUpdates = false,
        string? secretToken = null,
        CancellationToken cancellationToken = default)
    {
        return botClient
            .SetWebhook(
                url,
                certificate,
                ipAddress,
                maxConnections,
                allowedUpdates,
                dropPendingUpdates,
                secretToken,
                cancellationToken);
    }

    public Task SetMyCommands(
        IEnumerable<BotCommand> commands,
        BotCommandScope? scope = null,
        string? languageCode = null,
        CancellationToken cancellationToken = default)
    {
        return botClient
            .SetMyCommands(
                commands,
                scope,
                languageCode,
                cancellationToken);
    }

    public Task DeleteMyCommands(
        BotCommandScope? scope = null,
        string? languageCode = null,
        CancellationToken cancellationToken = default)
    {
        return botClient
            .DeleteMyCommands(
                scope,
                languageCode,
                cancellationToken);
    }

    public Task<TelegramUser> GetMe(CancellationToken cancellationToken = default)
    {
        return botClient
            .GetMe(cancellationToken);
    }

    public Task DeleteWebhook(
        bool dropPendingUpdates = false,
        CancellationToken cancellationToken = default)
    {
        return botClient
            .DeleteWebhook(dropPendingUpdates, cancellationToken);
    }
}