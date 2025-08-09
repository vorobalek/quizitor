using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;

namespace Quizitor.Bots.Behaviors.Infrastructure;

internal abstract partial class Behavior<TContext>
{
    public virtual bool ShouldPerformCallbackQueryDataPrefix(IBehaviorContext baseContext)
    {
        return this is ICallbackQueryDataPrefixBehaviorTrait<TContext> trait &&
               ICallbackQueryDataPrefixContext.IsValidUpdate(baseContext.UpdateContext.Update, trait.CallbackQueryDataPrefixValue);
    }

    public async Task<ICallbackQueryDataPrefixContext<TContext>?> PrepareCallbackQueryDataPrefixContextAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        var context = await PrepareAsync(baseContext, cancellationToken);
        return context is not null &&
               this is ICallbackQueryDataPrefixBehaviorTrait<TContext> trait
            ? ICallbackQueryDataPrefixContext.Create(context, trait.CallbackQueryDataPrefixValue)
            : null;
    }
}