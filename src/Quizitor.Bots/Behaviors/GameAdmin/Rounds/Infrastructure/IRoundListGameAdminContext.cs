using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.GameAdmin.Rounds.Infrastructure;

internal interface IRoundListGameAdminContext : IGameAdminContext
{
    Round[] Rounds { get; }
    int PageNumber { get; }
    int PageCount { get; }

    static IRoundListGameAdminContext Create(
        Round[] rounds,
        int pageNumber,
        int pageCount,
        IGameAdminContext baseContext)
    {
        return new RoundListGameAdminContext(
            rounds,
            pageNumber,
            pageCount,
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

    private record RoundListGameAdminContext(
        Round[] Rounds,
        int PageNumber,
        int PageCount,
        Game Game,
        Session Session,
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IRoundListGameAdminContext;
}