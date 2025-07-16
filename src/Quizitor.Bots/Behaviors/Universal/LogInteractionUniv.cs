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
internal sealed class LogInteractionUniv(ILogger<LogInteractionUniv> logger) :
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
        logger
            .LogWarning(
                "CLB {FromId} {Data}",
                context.CallbackQueryFromId,
                context.CallbackQueryDataPostfix);
        return Task.CompletedTask;
    }

    public Task PerformMessageTextAsync(
        MessageContext context,
        CancellationToken cancellationToken)
    {
        logger
            .LogWarning(
                "MSG {FromId} {Text}",
                context.MessageFromId,
                context.MessageText);
        return Task.CompletedTask;
    }

    public string QrCodeDataPrefixValue => string.Empty;

    public Task PerformQrCodeDataPrefixAsync(
        QrCodeContext context,
        CancellationToken cancellationToken)
    {
        logger
            .LogWarning(
                "QRC {FromId} {Text}",
                context.MessageFromId,
                context.QrCodeDataPostfix);
        return Task.CompletedTask;
    }
}