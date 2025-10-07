using Quizitor.Kafka.Contracts;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Api.Services.TelegramBot.ClientWrapper;

public interface ITelegramBotClientWrapper
{
    /// <inheritdoc cref="Telegram.Bot.TelegramBotClientExtensions.SendChatAction" />
    Task SendChatAction(
        UpdateContext updateContext,
        ChatId chatId,
        ChatAction action,
        int? messageThreadId = null,
        string? businessConnectionId = null,
        CancellationToken cancellationToken = default
    );

    /// <inheritdoc cref="Telegram.Bot.TelegramBotClientExtensions.SetWebhook" />
    Task SetWebhook(
        string url,
        InputFileStream? certificate = null,
        string? ipAddress = null,
        int? maxConnections = null,
        IEnumerable<UpdateType>? allowedUpdates = null,
        bool dropPendingUpdates = false,
        string? secretToken = null,
        CancellationToken cancellationToken = default
    );

    /// <inheritdoc cref="Telegram.Bot.TelegramBotClientExtensions.SetMyCommands" />
    Task SetMyCommands(
        IEnumerable<BotCommand> commands,
        BotCommandScope? scope = null,
        string? languageCode = null,
        CancellationToken cancellationToken = default
    );

    /// <inheritdoc cref="Telegram.Bot.TelegramBotClientExtensions.DeleteMyCommands" />
    Task DeleteMyCommands(
        BotCommandScope? scope = null,
        string? languageCode = null,
        CancellationToken cancellationToken = default
    );

    /// <inheritdoc cref="Telegram.Bot.TelegramBotClientExtensions.GetMe" />
    Task<TelegramUser> GetMe(CancellationToken cancellationToken = default);

    /// <inheritdoc cref="Telegram.Bot.TelegramBotClientExtensions.DeleteWebhook" />
    Task DeleteWebhook(
        bool dropPendingUpdates = false,
        CancellationToken cancellationToken = default
    );
}