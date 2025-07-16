using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.GameAdmin.Sessions.Internal;

internal interface ISessionJoinGameServerContext : ILoadBalancerContext
{
    Session ChosenSession { get; }
    Game ChosenGame { get; }
    bool IsExpired { get; }

    static ISessionJoinGameServerContext Create(
        Session chosenSession,
        Game chosenGame,
        bool isExpired,
        ILoadBalancerContext baseContext)
    {
        return new SessionJoinGameServerContext(
            chosenSession,
            chosenGame,
            isExpired,
            baseContext.TargetBot,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record SessionJoinGameServerContext(
        Session ChosenSession,
        Game ChosenGame,
        bool IsExpired,
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : ISessionJoinGameServerContext;
}