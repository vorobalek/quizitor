using LPlus;
using Quizitor.Data.Enums;

namespace Quizitor.Bots.Behaviors.Infrastructure;

internal interface IBehavior
{
    BotType Type { get; }

    string[] Permissions { get; }

    string Description => TR.L + $"_SHARED_BEHAVIOR_DESCRIPTION_{GetType().Name}";

    bool ShouldPerform(IBehaviorContext baseContext);

    Task PerformAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken);
}