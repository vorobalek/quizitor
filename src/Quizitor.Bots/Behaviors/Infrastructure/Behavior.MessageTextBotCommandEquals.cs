using Quizitor.Bots.Behaviors.Infrastructure.MessageTextBotCommandEquals;

namespace Quizitor.Bots.Behaviors.Infrastructure;

internal abstract partial class Behavior<TContext>
{
    public virtual bool ShouldPerformMessageTextBotCommandEquals(IBehaviorContext baseContext)
    {
        return this is IMessageTextBotCommandEqualsBehaviorTrait<TContext> trait &&
               IMessageTextBotCommandEqualsContext.IsValidUpdate(baseContext.UpdateContext.Update, trait.BotCommandValue);
    }

    public async Task<IMessageTextBotCommandEqualsContext<TContext>?> PrepareMessageTextBotCommandEqualsContextAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        var context = await PrepareAsync(baseContext, cancellationToken);
        return context is not null &&
               this is IMessageTextBotCommandEqualsBehaviorTrait<TContext> trait
            ? IMessageTextBotCommandEqualsContext.Create(context, trait.BotCommandValue)
            : null;
    }
}