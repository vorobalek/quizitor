using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.GameServer.Teams.Infrastructure;

internal interface ITeamSessionJoinGameServerContext : ILoadBalancerContext
{
    Session ChosenSession { get; }
    Team ChosenTeam { get; }
    Game ChosenGame { get; }
    bool IsExpired { get; }

    static ITeamSessionJoinGameServerContext Create(
        Session chosenSession,
        Team chosenTeam,
        Game chosenGame,
        bool isExpired,
        ILoadBalancerContext baseContext)
    {
        return new TeamSessionJoinGameServerContext(
            chosenSession,
            chosenTeam,
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

    private record TeamSessionJoinGameServerContext(
        Session ChosenSession,
        Team ChosenTeam,
        Game ChosenGame,
        bool IsExpired,
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : ITeamSessionJoinGameServerContext;
}