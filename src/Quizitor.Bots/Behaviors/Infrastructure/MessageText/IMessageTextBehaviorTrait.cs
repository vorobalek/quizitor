namespace Quizitor.Bots.Behaviors.Infrastructure.MessageText;

internal interface IMessageTextBehaviorTrait<TContext> :
    IBehaviorTrait<IMessageTextContext<TContext>>
    where TContext : IBehaviorContext
{
    bool IBehaviorTrait<IMessageTextContext<TContext>>.ShouldPerformSpecific(IBehaviorContext baseContext)
    {
        return ShouldPerformMessageText(baseContext);
    }

    Task<IMessageTextContext<TContext>?> IBehaviorTrait<IMessageTextContext<TContext>>.PrepareSpecificAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        return PrepareMessageTextContextAsync(
            baseContext,
            cancellationToken);
    }

    Task IBehaviorTrait<IMessageTextContext<TContext>>.PerformSpecificAsync(
        IMessageTextContext<TContext> context,
        CancellationToken cancellationToken)
    {
        return PerformMessageTextAsync(
            context,
            cancellationToken);
    }

    bool ShouldPerformMessageText(IBehaviorContext baseContext);

    Task<IMessageTextContext<TContext>?> PrepareMessageTextContextAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken);

    Task PerformMessageTextAsync(
        IMessageTextContext<TContext> context,
        CancellationToken cancellationToken);
}