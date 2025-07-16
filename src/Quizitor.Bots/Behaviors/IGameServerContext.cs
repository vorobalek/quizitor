using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors;

internal interface ISessionTeamInfo
{
    Team Team { get; }
    User? Leader { get; }
    User[] Members { get; }
    User[] OfflineMembers { get; }

    static ISessionTeamInfo Create(
        Team team,
        User? leader,
        User[] members,
        User[] offlineMembers)
    {
        return new SessionTeamInfo(
            team,
            leader,
            members,
            offlineMembers);
    }

    private record SessionTeamInfo(
        Team Team,
        User? Leader,
        User[] Members,
        User[] OfflineMembers) : ISessionTeamInfo;
}

internal interface IGameServerContext : ILoadBalancerContext
{
    ISessionTeamInfo? SessionTeamInfo { get; }
    public Game Game { get; }
    public Session Session { get; }

    static IGameServerContext Create(
        ISessionTeamInfo? teamInfo,
        Game game,
        Session session,
        ILoadBalancerContext baseContext)
    {
        return new GameServerContext(
            teamInfo,
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

    private record GameServerContext(
        ISessionTeamInfo? SessionTeamInfo,
        Game Game,
        Session Session,
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IGameServerContext;
}