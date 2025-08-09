using Quizitor.Bots.Behaviors.Infrastructure.QrCodeDataPrefix;

namespace Quizitor.Bots.Behaviors.Infrastructure;

internal abstract partial class Behavior<TContext>
{
    public virtual bool ShouldPerformQrCodeDataPrefix(IBehaviorContext baseContext)
    {
        return this is IQrCodeDataPrefixBehaviorTrait<TContext> trait &&
               IQrCodeDataPrefixContext.IsValidUpdate(baseContext.UpdateContext.Update, baseContext.QrData, trait.QrCodeDataPrefixValue);
    }

    public async Task<IQrCodeDataPrefixContext<TContext>?> PrepareQrCodeDataPrefixContextAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        var context = await PrepareAsync(baseContext, cancellationToken);
        return context is not null &&
               this is IQrCodeDataPrefixBehaviorTrait<TContext> trait
            ? IQrCodeDataPrefixContext.Create(context, trait.QrCodeDataPrefixValue)
            : null;
    }
}