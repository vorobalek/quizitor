using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Data.Enums;

namespace Quizitor.Bots.Behaviors;

internal abstract class UniversalBehavior<TContext> :
    Behavior<TContext>
    where TContext : IBehaviorContext
{
    public override BotType Type => BotType.Universal;
}

internal abstract class UniversalBehavior :
    UniversalBehavior<IBehaviorContext>
{
    protected override bool ShouldPerformInternal(IBehaviorContext baseContext)
    {
        return false;
    }

    protected override Task<IBehaviorContext?> PrepareInternalAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IBehaviorContext?>(baseContext);
    }
}