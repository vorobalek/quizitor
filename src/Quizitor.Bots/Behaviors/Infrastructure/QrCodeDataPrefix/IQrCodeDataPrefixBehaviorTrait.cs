namespace Quizitor.Bots.Behaviors.Infrastructure.QrCodeDataPrefix;

internal interface IQrCodeDataPrefixBehaviorTrait<TContext> :
    IBehaviorTrait<IQrCodeDataPrefixContext<TContext>>
    where TContext : IBehaviorContext
{
    string QrCodeDataPrefixValue { get; }

    bool IBehaviorTrait<IQrCodeDataPrefixContext<TContext>>.ShouldPerformSpecific(IBehaviorContext baseContext)
    {
        return ShouldPerformQrCodeDataPrefix(baseContext);
    }

    Task<IQrCodeDataPrefixContext<TContext>?> IBehaviorTrait<IQrCodeDataPrefixContext<TContext>>.PrepareSpecificAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        return PrepareQrCodeDataPrefixContextAsync(
            baseContext,
            cancellationToken);
    }

    Task IBehaviorTrait<IQrCodeDataPrefixContext<TContext>>.PerformSpecificAsync(
        IQrCodeDataPrefixContext<TContext> context,
        CancellationToken cancellationToken)
    {
        return PerformQrCodeDataPrefixAsync(context, cancellationToken);
    }

    bool ShouldPerformQrCodeDataPrefix(IBehaviorContext baseContext);

    Task<IQrCodeDataPrefixContext<TContext>?> PrepareQrCodeDataPrefixContextAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken);

    Task PerformQrCodeDataPrefixAsync(
        IQrCodeDataPrefixContext<TContext> context,
        CancellationToken cancellationToken);
}