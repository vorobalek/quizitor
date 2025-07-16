using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors;

internal interface IGameAdminContext : ILoadBalancerContext
{
    public Game Game { get; }
    public Session Session { get; }

    static IGameAdminContext Create(
        Game game,
        Session session,
        ILoadBalancerContext baseContext)
    {
        return new GameAdminContext(
            game,
            session,
            baseContext.TargetBot,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record GameAdminContext(
        Game Game,
        Session Session,
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IGameAdminContext;
}