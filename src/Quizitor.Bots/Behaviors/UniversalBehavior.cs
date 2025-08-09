using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Data.Enums;

namespace Quizitor.Bots.Behaviors;

internal abstract class UniversalBehavior<TContext> :
    Behavior<TContext>
    where TContext : IBehaviorContext
{
    public sealed override BotType Type => BotType.Universal;

    protected sealed override bool ShouldPerformInternal(IBehaviorContext baseContext)
    {
        return true;
    }
}

internal abstract class UniversalBehavior :
    UniversalBehavior<IBehaviorContext>
{
    protected sealed override Task<IBehaviorContext?> PrepareInternalAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IBehaviorContext?>(baseContext);
    }
}