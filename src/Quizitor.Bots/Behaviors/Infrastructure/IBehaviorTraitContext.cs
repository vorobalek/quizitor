namespace Quizitor.Bots.Behaviors.Infrastructure;

internal interface IBehaviorTraitContext<out TBase>
    where TBase : IBehaviorContext
{
    TBase Base { get; }
}