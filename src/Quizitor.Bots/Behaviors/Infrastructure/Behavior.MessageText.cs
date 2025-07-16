using Quizitor.Bots.Behaviors.Infrastructure.MessageText;

namespace Quizitor.Bots.Behaviors.Infrastructure;

internal abstract partial class Behavior<TContext>
{
    public virtual bool ShouldPerformMessageText(IBehaviorContext baseContext)
    {
        return this is IMessageTextBehaviorTrait<TContext> &&
               IMessageTextContext.IsValidUpdate(baseContext.UpdateContext.Update);
    }

    public virtual async Task<IMessageTextContext<TContext>?> PrepareMessageTextContextAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        var context = await PrepareAsync(baseContext, cancellationToken);
        return context is not null &&
               this is IMessageTextBehaviorTrait<TContext>
            ? IMessageTextContext.Create(context)
            : null;
    }
}