using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.Infrastructure.QrCodeDataPrefix;

internal interface IQrCodeDataPrefixContext
{
    long MessageFromId { get; }
    string QrCodeDataPostfix { get; }

    static bool IsValidUpdate(
        Update update,
        string? qrCodeData,
        string qrCodeDataPrefixValue)
    {
        return
            update is
            {
                Type: UpdateType.Message,
                Message:
                {
                    From: not null,
                    Photo.Length: > 0
                }
            } &&
            qrCodeData is not null &&
            qrCodeData.StartsWith(qrCodeDataPrefixValue);
    }

    static IQrCodeDataPrefixContext<TContext>? Create<TContext>(
        TContext baseContext,
        string qrCodeDataPrefixValue)
        where TContext : IBehaviorContext
    {
        return IsValidUpdate(baseContext.UpdateContext.Update, baseContext.QrData, qrCodeDataPrefixValue)
            ? new QrCodeDataPrefixContext<TContext>(
                baseContext,
                baseContext.UpdateContext.Update.Message!.From!.Id,
                baseContext.QrData![qrCodeDataPrefixValue.Length ..])
            : null;
    }

    private record QrCodeDataPrefixContext<TContext>(
        TContext Base,
        long MessageFromId,
        string QrCodeDataPostfix) : IQrCodeDataPrefixContext<TContext>
        where TContext : IBehaviorContext;
}

internal interface IQrCodeDataPrefixContext<out TContext> :
    IBehaviorTraitContext<TContext>,
    IQrCodeDataPrefixContext
    where TContext : IBehaviorContext;