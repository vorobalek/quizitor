using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Bots.Infrastructure;

internal interface IBotBackOfficeContext : IBackOfficeContext
{
    Bot Bot { get; }
    int GameServerUsersCount { get; }
    int GameAdminUsersCount { get; }
    int BotPageNumber { get; }

    static IBotBackOfficeContext Create(
        Bot bot,
        int gameServerUsersCount,
        int gameAdminUsersCount,
        int botPageNumber,
        IBackOfficeContext baseContext)
    {
        return new BotBackOfficeContext(
            bot,
            gameServerUsersCount,
            gameAdminUsersCount,
            botPageNumber,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record BotBackOfficeContext(
        Bot Bot,
        int GameServerUsersCount,
        int GameAdminUsersCount,
        int BotPageNumber,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IBotBackOfficeContext;
}