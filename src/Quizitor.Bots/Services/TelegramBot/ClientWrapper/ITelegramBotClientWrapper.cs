using Quizitor.Kafka.Contracts;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Services.TelegramBot.ClientWrapper;

internal interface ITelegramBotClientWrapper
{
    /// <inheritdoc cref="Telegram.Bot.TelegramBotClientExtensions.SendMessage" />
    Task SendMessage(
        UpdateContext updateContext,
        ChatId chatId,
        string text,
        ParseMode parseMode = default,
        ReplyParameters? replyParameters = null,
        ReplyMarkup? replyMarkup = null,
        LinkPreviewOptions? linkPreviewOptions = null,
        int? messageThreadId = null,
        IEnumerable<MessageEntity>? entities = null,
        bool disableNotification = false,
        bool protectContent = false,
        string? messageEffectId = null,
        string? businessConnectionId = null,
        bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default);

    /// <inheritdoc
    ///     cref="Telegram.Bot.TelegramBotClientExtensions.EditMessageText(ITelegramBotClient, ChatId, int, string, ParseMode, IEnumerable{MessageEntity}?, LinkPreviewOptions?, InlineKeyboardMarkup?, string?, CancellationToken)" />
    Task EditMessageText(
        UpdateContext updateContext,
        ChatId chatId,
        int messageId,
        string text,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? entities = null,
        LinkPreviewOptions? linkPreviewOptions = null,
        InlineKeyboardMarkup? replyMarkup = null,
        string? businessConnectionId = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="Telegram.Bot.TelegramBotClientExtensions.AnswerCallbackQuery" />
    Task AnswerCallbackQuery(
        UpdateContext updateContext,
        string callbackQueryId,
        string? text = null,
        bool showAlert = false,
        string? url = null,
        int? cacheTime = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc cref="Telegram.Bot.TelegramBotClientExtensions.DeleteMessage" />
    Task DeleteMessage(
        UpdateContext updateContext,
        ChatId chatId,
        int messageId,
        CancellationToken cancellationToken = default);


    /// <inheritdoc cref="Telegram.Bot.TelegramBotClientExtensions.SendChatAction" />
    Task SendChatAction(
        UpdateContext updateContext,
        ChatId chatId,
        ChatAction action,
        int? messageThreadId = null,
        string? businessConnectionId = null,
        CancellationToken cancellationToken = default
    );

    /// <inheritdoc cref="Telegram.Bot.TelegramBotClientExtensions.SendPhoto" />
    Task SendPhoto(
        UpdateContext updateContext,
        ChatId chatId,
        InputFile photo,
        string? caption = null,
        ParseMode parseMode = default,
        ReplyParameters? replyParameters = null,
        int? messageThreadId = null,
        IEnumerable<MessageEntity>? captionEntities = null,
        bool showCaptionAboveMedia = false,
        bool hasSpoiler = false,
        bool disableNotification = false,
        bool protectContent = false,
        string? messageEffectId = null,
        string? businessConnectionId = null,
        bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default
    );

    /// <inheritdoc cref="Telegram.Bot.TelegramBotClientExtensions.SetMyCommands" />
    Task SetMyCommands(
        IEnumerable<BotCommand> commands,
        BotCommandScope? scope = null,
        string? languageCode = null,
        CancellationToken cancellationToken = default
    );

    /// <inheritdoc cref="Telegram.Bot.TelegramBotClientExtensions.GetMe" />
    Task<TelegramUser> GetMe(CancellationToken cancellationToken = default);

    /// <inheritdoc cref="Telegram.Bot.TelegramBotClientExtensions.GetFile" />
    Task<TGFile> GetFile(string fileId, CancellationToken cancellationToken = default);

    /// <inheritdoc cref="Telegram.Bot.ITelegramBotClient.DownloadFile(string, Stream, CancellationToken)" />
    Task DownloadFile(string filePath, Stream destination, CancellationToken cancellationToken = default);
}