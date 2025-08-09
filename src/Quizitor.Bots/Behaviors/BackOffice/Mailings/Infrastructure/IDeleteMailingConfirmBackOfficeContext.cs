using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;

internal interface IDeleteMailingConfirmBackOfficeContext : IMailingListBackOfficeContext
{
    Mailing Mailing { get; }

    static IDeleteMailingConfirmBackOfficeContext Create(
        Mailing mailing,
        Mailing[] mailings,
        int mailingsCount,
        int mailingPageNumber,
        int mailingPageCount,
        IBackOfficeContext baseContext)
    {
        return new DeleteMailingConfirmBackOfficeContext(
            mailing,
            mailings,
            mailingsCount,
            mailingPageNumber,
            mailingPageCount,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record DeleteMailingConfirmBackOfficeContext(
        Mailing Mailing,
        Mailing[] Mailings,
        int MailingsCount,
        int MailingPageNumber,
        int MailingPageCount,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IDeleteMailingConfirmBackOfficeContext;
}