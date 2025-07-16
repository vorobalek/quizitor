using Quizitor.Bots.Services.Kafka.Producers.AnswerCallbackQuery;
using Quizitor.Bots.Services.Kafka.Producers.DeleteMessage;
using Quizitor.Bots.Services.Kafka.Producers.EditMessage;
using Quizitor.Bots.Services.Kafka.Producers.SendChatAction;
using Quizitor.Bots.Services.Kafka.Producers.SendMessage;
using Quizitor.Bots.Services.Kafka.Producers.SendPhoto;
using Quizitor.Kafka.Contracts;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Services.TelegramBot.ClientWrapper;

internal sealed class TelegramBotClientWrapper(
    int? botId,
    ITelegramBotClient botClient,
    ISendMessageKafkaProducer sendMessageKafkaProducer,
    IEditMessageKafkaProducer editMessageKafkaProducer,
    IAnswerCallbackQueryKafkaProducer answerCallbackQueryKafkaProducer,
    IDeleteMessageKafkaProducer deleteMessageKafkaProducer,
    ISendChatActionKafkaProducer sendChatActionKafkaProducer,
    ISendPhotoKafkaProducer sendPhotoKafkaProducer)
    : ITelegramBotClientWrapper
{
    public Task SendMessage(
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
        CancellationToken cancellationToken = default)
    {
        var message = new SendMessageRequest
        {
            ChatId = chatId,
            Text = text,
            ParseMode = parseMode,
            ReplyParameters = replyParameters,
            ReplyMarkup = replyMarkup,
            LinkPreviewOptions = linkPreviewOptions,
            MessageThreadId = messageThreadId,
            Entities = entities,
            DisableNotification = disableNotification,
            ProtectContent = protectContent,
            MessageEffectId = messageEffectId,
            BusinessConnectionId = businessConnectionId,
            AllowPaidBroadcast = allowPaidBroadcast
        };

        return botId is null
            ? sendMessageKafkaProducer.ProduceAsync(
                message,
                updateContext,
                cancellationToken)
            : sendMessageKafkaProducer.ProduceBotAsync(
                botId.Value,
                message,
                updateContext,
                cancellationToken);
    }

    public Task EditMessageText(
        UpdateContext updateContext,
        ChatId chatId,
        int messageId,
        string text,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? entities = null,
        LinkPreviewOptions? linkPreviewOptions = null,
        InlineKeyboardMarkup? replyMarkup = null,
        string? businessConnectionId = null,
        CancellationToken cancellationToken = default)
    {
        var message = new EditMessageTextRequest
        {
            ChatId = chatId,
            MessageId = messageId,
            Text = text,
            ParseMode = parseMode,
            Entities = entities,
            LinkPreviewOptions = linkPreviewOptions,
            ReplyMarkup = replyMarkup,
            BusinessConnectionId = businessConnectionId
        };

        return botId is null
            ? editMessageKafkaProducer.ProduceAsync(
                message,
                updateContext,
                cancellationToken)
            : editMessageKafkaProducer.ProduceBotAsync(
                botId.Value,
                message,
                updateContext,
                cancellationToken);
    }

    public Task AnswerCallbackQuery(
        UpdateContext updateContext,
        string callbackQueryId,
        string? text = null,
        bool showAlert = false,
        string? url = null,
        int? cacheTime = null,
        CancellationToken cancellationToken = default)
    {
        var message = new AnswerCallbackQueryRequest
        {
            CallbackQueryId = callbackQueryId,
            Text = text,
            ShowAlert = showAlert,
            Url = url,
            CacheTime = cacheTime
        };

        return botId is null
            ? answerCallbackQueryKafkaProducer.ProduceAsync(
                message,
                updateContext,
                cancellationToken)
            : answerCallbackQueryKafkaProducer.ProduceBotAsync(
                botId.Value,
                message,
                updateContext,
                cancellationToken);
    }

    public Task DeleteMessage(
        UpdateContext updateContext,
        ChatId chatId,
        int messageId,
        CancellationToken cancellationToken = default)
    {
        var message = new DeleteMessageRequest
        {
            ChatId = chatId,
            MessageId = messageId
        };

        return botId is null
            ? deleteMessageKafkaProducer.ProduceAsync(
                message,
                updateContext,
                cancellationToken)
            : deleteMessageKafkaProducer.ProduceBotAsync(
                botId.Value,
                message,
                updateContext,
                cancellationToken);
    }

    public Task SendChatAction(
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

        return botId is null
            ? sendChatActionKafkaProducer.ProduceAsync(
                message,
                updateContext,
                cancellationToken)
            : sendChatActionKafkaProducer.ProduceBotAsync(
                botId.Value,
                message,
                updateContext,
                cancellationToken);
    }

    public Task SendPhoto(
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
        CancellationToken cancellationToken = default)
    {
        var message = new SendPhotoRequest
        {
            ChatId = chatId,
            Photo = photo,
            Caption = caption,
            ParseMode = parseMode,
            ReplyParameters = replyParameters,
            ReplyMarkup = null,
            MessageThreadId = messageThreadId,
            CaptionEntities = captionEntities,
            ShowCaptionAboveMedia = showCaptionAboveMedia,
            HasSpoiler = hasSpoiler,
            DisableNotification = disableNotification,
            ProtectContent = protectContent,
            MessageEffectId = messageEffectId,
            BusinessConnectionId = businessConnectionId,
            AllowPaidBroadcast = allowPaidBroadcast
        };

        return botId is null
            ? sendPhotoKafkaProducer.ProduceAsync(
                message,
                updateContext,
                cancellationToken)
            : sendPhotoKafkaProducer.ProduceBotAsync(
                botId.Value,
                message,
                updateContext,
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

    public Task<TelegramUser> GetMe(CancellationToken cancellationToken = default)
    {
        return botClient
            .GetMe(cancellationToken);
    }

    public Task<TGFile> GetFile(
        string fileId,
        CancellationToken cancellationToken = default)
    {
        return botClient
            .GetFile(
                fileId,
                cancellationToken);
    }

    public Task DownloadFile(
        string filePath,
        Stream destination,
        CancellationToken cancellationToken = default)
    {
        return botClient
            .DownloadFile(
                filePath,
                destination,
                cancellationToken);
    }
}