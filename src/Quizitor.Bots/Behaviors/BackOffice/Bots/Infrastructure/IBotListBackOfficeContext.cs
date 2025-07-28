using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Bots.Infrastructure;

internal interface IBotListBackOfficeContext : IBackOfficeContext
{
    Bot[] Bots { get; }
    int BotsCount { get; }
    int BotPageNumber { get; }
    int BotPageCount { get; }

    static IBotListBackOfficeContext Create(
        Bot[] bots,
        int botsCount,
        int botPageNumber,
        int botPageCount,
        IBackOfficeContext baseContext)
    {
        return new BotListBackOfficeContext(
            bots,
            botsCount,
            botPageNumber,
            botPageCount,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record BotListBackOfficeContext(
        Bot[] Bots,
        int BotsCount,
        int BotPageNumber,
        int BotPageCount,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IBotListBackOfficeContext;
}