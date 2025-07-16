using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;

internal interface IMailingListBackOfficeContext : IBackOfficeContext
{
    Mailing[] Mailings { get; }
    int MailingPageNumber { get; }
    int MailingPageCount { get; }

    static IMailingListBackOfficeContext Create(
        Mailing[] mailings,
        int mailingPageNumber,
        int mailingPageCount,
        IBackOfficeContext baseContext)
    {
        return new MailingListBackOfficeContext(
            mailings,
            mailingPageNumber,
            mailingPageCount,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record MailingListBackOfficeContext(
        Mailing[] Mailings,
        int MailingPageNumber,
        int MailingPageCount,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IMailingListBackOfficeContext;
}