using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Behaviors.Infrastructure.MessageText;
using Quizitor.Bots.Behaviors.Infrastructure.QrCodeDataPrefix;

namespace Quizitor.Bots.Behaviors.Universal;

using CallbackQueryBehavior = ICallbackQueryDataPrefixBehaviorTrait<IBehaviorContext>;
using CallbackQueryContext = ICallbackQueryDataPrefixContext<IBehaviorContext>;
using MessageBehavior = IMessageTextBehaviorTrait<IBehaviorContext>;
using MessageContext = IMessageTextContext<IBehaviorContext>;
using QrCodeBehavior = IQrCodeDataPrefixBehaviorTrait<IBehaviorContext>;
using QrCodeContext = IQrCodeDataPrefixContext<IBehaviorContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed partial class LogInteractionUniv(ILogger<LogInteractionUniv> logger) :
    UniversalBehavior,
    MessageBehavior,
    CallbackQueryBehavior,
    QrCodeBehavior
{
    public override string[] Permissions => [];

    public string CallbackQueryDataPrefixValue => string.Empty;

    public Task PerformCallbackQueryDataPrefixAsync(
        CallbackQueryContext context,
        CancellationToken cancellationToken)
    {
        LogCallback(
            logger,
            context.Base.EntryBot?.Username,
            context.CallbackQueryFromId,
            context.CallbackQueryDataPostfix);
        return Task.CompletedTask;
    }

    public Task PerformMessageTextAsync(
        MessageContext context,
        CancellationToken cancellationToken)
    {
        LogMessage(
            logger,
            context.Base.EntryBot?.Username,
            context.MessageFromId,
            context.MessageText);
        return Task.CompletedTask;
    }

    public string QrCodeDataPrefixValue => string.Empty;

    public Task PerformQrCodeDataPrefixAsync(
        QrCodeContext context,
        CancellationToken cancellationToken)
    {
        LogQrCode(
            logger,
            context.Base.EntryBot?.Username,
            context.MessageFromId,
            context.QrCodeDataPostfix);
        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Warning, "CLB {username} {fromId} {data}")]
    static partial void LogCallback(ILogger<LogInteractionUniv> logger, string? username, long fromId, string data);

    [LoggerMessage(LogLevel.Warning, "MSG {username} {fromId} {text}")]
    static partial void LogMessage(ILogger<LogInteractionUniv> logger, string? username, long fromId, string text);

    [LoggerMessage(LogLevel.Warning, "QRC {username} {fromId} {data}")]
    static partial void LogQrCode(ILogger<LogInteractionUniv> logger, string? username, long fromId, string data);
}