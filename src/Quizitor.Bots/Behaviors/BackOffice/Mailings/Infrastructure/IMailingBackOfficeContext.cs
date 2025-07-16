using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;

internal interface IMailingBackOfficeContext : IBackOfficeContext
{
    Mailing Mailing { get; }
    int MailingPageNumber { get; }

    static IMailingBackOfficeContext Create(
        Mailing mailing,
        int mailingPageNumber,
        IBackOfficeContext baseContext)
    {
        return new MailingBackOfficeContext(
            mailing,
            mailingPageNumber,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record MailingBackOfficeContext(
        Mailing Mailing,
        int MailingPageNumber,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IMailingBackOfficeContext;
}