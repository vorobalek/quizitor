namespace Quizitor.Bots.Behaviors.Infrastructure.MessageTextBotCommandEquals;

internal interface IMessageTextBotCommandEqualsBehaviorTrait<TContext> :
    IBehaviorTrait<IMessageTextBotCommandEqualsContext<TContext>>
    where TContext : IBehaviorContext
{
    string BotCommandValue { get; }

    bool IBehaviorTrait<IMessageTextBotCommandEqualsContext<TContext>>.ShouldPerformSpecific(IBehaviorContext baseContext)
    {
        return ShouldPerformMessageTextBotCommandEquals(baseContext);
    }

    Task<IMessageTextBotCommandEqualsContext<TContext>?> IBehaviorTrait<IMessageTextBotCommandEqualsContext<TContext>>.PrepareSpecificAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        return PrepareMessageTextBotCommandEqualsContextAsync(
            baseContext,
            cancellationToken);
    }

    Task IBehaviorTrait<IMessageTextBotCommandEqualsContext<TContext>>.PerformSpecificAsync(
        IMessageTextBotCommandEqualsContext<TContext> context,
        CancellationToken cancellationToken)
    {
        return PerformMessageTextBotCommandEqualsAsync(
            context,
            cancellationToken);
    }

    bool ShouldPerformMessageTextBotCommandEquals(IBehaviorContext baseContext);

    Task<IMessageTextBotCommandEqualsContext<TContext>?> PrepareMessageTextBotCommandEqualsContextAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken);

    Task PerformMessageTextBotCommandEqualsAsync(
        IMessageTextBotCommandEqualsContext<TContext> context,
        CancellationToken cancellationToken);
}