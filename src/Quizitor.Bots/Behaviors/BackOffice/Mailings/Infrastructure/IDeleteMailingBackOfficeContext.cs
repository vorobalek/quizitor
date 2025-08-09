using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;

internal interface IDeleteMailingBackOfficeContext : IBackOfficeContext
{
    Mailing Mailing { get; }
    int MailingPageNumber { get; }

    static IDeleteMailingBackOfficeContext Create(
        Mailing mailing,
        int mailingPageNumber,
        IBackOfficeContext baseContext)
    {
        return new DeleteMailingBackOfficeContext(
            mailing,
            mailingPageNumber,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record DeleteMailingBackOfficeContext(
        Mailing Mailing,
        int MailingPageNumber,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IDeleteMailingBackOfficeContext;
}