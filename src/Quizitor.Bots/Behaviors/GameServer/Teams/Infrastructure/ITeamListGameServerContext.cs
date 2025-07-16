using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.GameServer.Teams.Infrastructure;

internal interface ITeamListGameServerContext : IGameServerContext
{
    Team[] Teams { get; }
    int TeamPageNumber { get; }
    int TeamPageCount { get; }

    static ITeamListGameServerContext Create(
        Team[] teams,
        int teamPageNumber,
        int teamPageCount,
        IGameServerContext baseContext)
    {
        return new TeamListGameServerContext(
            teams,
            teamPageNumber,
            teamPageCount,
            baseContext.SessionTeamInfo,
            baseContext.Game,
            baseContext.Session,
            baseContext.TargetBot,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record TeamListGameServerContext(
        Team[] Teams,
        int TeamPageNumber,
        int TeamPageCount,
        ISessionTeamInfo? SessionTeamInfo,
        Game Game,
        Session Session,
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : ITeamListGameServerContext;
}