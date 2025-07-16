namespace Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;

internal interface ICallbackQueryDataPrefixBehaviorTrait<TContext> :
    IBehaviorTrait<ICallbackQueryDataPrefixContext<TContext>>
    where TContext : IBehaviorContext
{
    string CallbackQueryDataPrefixValue { get; }

    bool IBehaviorTrait<ICallbackQueryDataPrefixContext<TContext>>.ShouldPerformSpecific(IBehaviorContext baseContext)
    {
        return ShouldPerformCallbackQueryDataPrefix(baseContext);
    }

    Task<ICallbackQueryDataPrefixContext<TContext>?> IBehaviorTrait<ICallbackQueryDataPrefixContext<TContext>>.PrepareSpecificAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        return PrepareCallbackQueryDataPrefixContextAsync(
            baseContext,
            cancellationToken);
    }

    Task IBehaviorTrait<ICallbackQueryDataPrefixContext<TContext>>.PerformSpecificAsync(
        ICallbackQueryDataPrefixContext<TContext> context,
        CancellationToken cancellationToken)
    {
        return PerformCallbackQueryDataPrefixAsync(context, cancellationToken);
    }

    bool ShouldPerformCallbackQueryDataPrefix(IBehaviorContext baseContext);

    Task<ICallbackQueryDataPrefixContext<TContext>?> PrepareCallbackQueryDataPrefixContextAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken);

    Task PerformCallbackQueryDataPrefixAsync(
        ICallbackQueryDataPrefixContext<TContext> context,
        CancellationToken cancellationToken);
}