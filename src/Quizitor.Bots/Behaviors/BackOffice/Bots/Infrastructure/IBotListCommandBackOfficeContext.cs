using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Bots.Infrastructure;

internal interface IBotListCommandBackOfficeContext : IBotListBackOfficeContext
{
    Bot Bot { get; }

    static IBotListCommandBackOfficeContext Create(
        Bot bot,
        IBotListBackOfficeContext baseContext)
    {
        return new BotListCommandBackOfficeContext(
            bot,
            baseContext.Bots,
            baseContext.BotsCount,
            baseContext.BotPageNumber,
            baseContext.BotPageCount,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record BotListCommandBackOfficeContext(
        Bot Bot,
        Bot[] Bots,
        int BotsCount,
        int BotPageNumber,
        int BotPageCount,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IBotListCommandBackOfficeContext;
}