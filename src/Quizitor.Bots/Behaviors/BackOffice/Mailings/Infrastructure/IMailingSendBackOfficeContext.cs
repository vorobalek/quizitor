using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;

internal interface IMailingSendBackOfficeContext : IBackOfficeContext
{
    Mailing Mailing { get; }
    IGrouping<long, int>[] UsersBots { get; }

    static IMailingSendBackOfficeContext Create(
        Mailing mailing,
        IGrouping<long, int>[] usersBots,
        IBackOfficeContext baseContext)
    {
        return new MailingSendBackOfficeContext(
            mailing,
            usersBots,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record MailingSendBackOfficeContext(
        Mailing Mailing,
        IGrouping<long, int>[] UsersBots,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IMailingSendBackOfficeContext;
}