using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors;

internal interface ILoadBalancerContext : IBehaviorContext
{
    public Bot? TargetBot { get; }

    static ILoadBalancerContext Create(
        Bot? targetBot,
        IBehaviorContext baseContext)
    {
        return new LoadBalancerContext(
            targetBot,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record LoadBalancerContext(
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : ILoadBalancerContext;
}