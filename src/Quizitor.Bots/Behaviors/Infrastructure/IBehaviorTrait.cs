namespace Quizitor.Bots.Behaviors.Infrastructure;

internal interface IBehaviorTrait<TContext>
{
    bool ShouldPerformSpecific(IBehaviorContext baseContext);

    Task<TContext?> PrepareSpecificAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken);

    Task PerformSpecificAsync(
        TContext context,
        CancellationToken cancellationToken);
}