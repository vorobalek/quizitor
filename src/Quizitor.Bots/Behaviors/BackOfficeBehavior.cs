using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Data.Enums;

namespace Quizitor.Bots.Behaviors;

internal abstract class BackOfficeBehavior<TContext> :
    Behavior<TContext>
    where TContext : IBackOfficeContext
{
    public override BotType Type => BotType.BackOffice;

    protected override Task<TContext?> PrepareInternalAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        var backOfficeContext = IBackOfficeContext.Create(baseContext);
        return PrepareContextAsync(backOfficeContext, cancellationToken);
    }

    protected abstract Task<TContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken);
}

internal abstract class BackOfficeBehavior :
    BackOfficeBehavior<IBackOfficeContext>
{
    protected override Task<IBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IBackOfficeContext?>(backOfficeContext);
    }
}