using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Bots.Behaviors;

internal abstract class LoadBalancerBehavior<TContext>(IDbContextProvider dbContextProvider) :
    Behavior<TContext>
    where TContext : ILoadBalancerContext
{
    public override BotType Type => BotType.LoadBalancer;
    protected abstract BotType UserTargetBotType { get; }
    protected abstract int? GetUserTargetBotId(User user);
    protected abstract Task<Bot?> GetTargetBotAsync(CancellationToken cancellationToken);

    protected sealed override async Task<TContext?> PrepareInternalAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        var userTargetBotId = GetUserTargetBotId(baseContext.Identity.User);
        var targetBot = await dbContextProvider
            .Bots
            .GetByIdOrDefaultAsync(
                userTargetBotId,
                cancellationToken);

        targetBot = targetBot is
            { IsActive: true, Username: not null } && targetBot.Type == UserTargetBotType
            ? targetBot
            : await GetTargetBotAsync(cancellationToken);

        return await RedirectIfNeededAsync(targetBot, baseContext, cancellationToken)
            ? default
            : await PrepareLoadBalancerInternalAsync(targetBot, baseContext, cancellationToken);
    }

    protected abstract Task<bool> RedirectIfNeededAsync(Bot? targetBot, IBehaviorContext baseContext, CancellationToken cancellationToken);

    protected abstract Task<TContext?> PrepareLoadBalancerInternalAsync(Bot? targetBot, IBehaviorContext baseContext, CancellationToken cancellationToken);
}