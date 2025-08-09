using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;

namespace Quizitor.Bots.Behaviors.Infrastructure;

internal abstract partial class Behavior<TContext>
{
    public virtual bool ShouldPerformCallbackQueryDataEquals(IBehaviorContext baseContext)
    {
        return this is ICallbackQueryDataEqualsBehaviorTrait<TContext> trait &&
               ICallbackQueryDataEqualsContext.IsValidUpdate(baseContext.UpdateContext.Update, trait.CallbackQueryDataValue);
    }

    public async Task<ICallbackQueryDataEqualsContext<TContext>?> PrepareCallbackQueryDataEqualsContextAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        var context = await PrepareAsync(baseContext, cancellationToken);
        return context is not null &&
               this is ICallbackQueryDataEqualsBehaviorTrait<TContext> trait
            ? ICallbackQueryDataEqualsContext.Create(context, trait.CallbackQueryDataValue)
            : null;
    }
}