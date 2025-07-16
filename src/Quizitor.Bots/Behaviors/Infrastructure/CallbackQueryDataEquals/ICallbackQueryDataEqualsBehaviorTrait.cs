namespace Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;

internal interface ICallbackQueryDataEqualsBehaviorTrait<TContext> :
    IBehaviorTrait<ICallbackQueryDataEqualsContext<TContext>>
    where TContext : IBehaviorContext
{
    string CallbackQueryDataValue { get; }

    bool IBehaviorTrait<ICallbackQueryDataEqualsContext<TContext>>.ShouldPerformSpecific(IBehaviorContext baseContext)
    {
        return ShouldPerformCallbackQueryDataEquals(baseContext);
    }

    Task<ICallbackQueryDataEqualsContext<TContext>?> IBehaviorTrait<ICallbackQueryDataEqualsContext<TContext>>.PrepareSpecificAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        return PrepareCallbackQueryDataEqualsContextAsync(
            baseContext,
            cancellationToken);
    }

    Task IBehaviorTrait<ICallbackQueryDataEqualsContext<TContext>>.PerformSpecificAsync(
        ICallbackQueryDataEqualsContext<TContext> context,
        CancellationToken cancellationToken)
    {
        return PerformCallbackQueryDataEqualsAsync(
            context,
            cancellationToken);
    }

    bool ShouldPerformCallbackQueryDataEquals(IBehaviorContext baseContext);

    Task<ICallbackQueryDataEqualsContext<TContext>?> PrepareCallbackQueryDataEqualsContextAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken);

    Task PerformCallbackQueryDataEqualsAsync(
        ICallbackQueryDataEqualsContext<TContext> context,
        CancellationToken cancellationToken);
}