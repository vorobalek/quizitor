using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Infrastructure;

internal interface IMainPageBackOfficeContext : IBackOfficeContext
{
    int BotsCount { get; }
    int UsersCount { get; }
    int MailingsCount { get; }
    int GamesCount { get; }

    static IMainPageBackOfficeContext Create(
        int botsCount,
        int usersCount,
        int mailingsCount,
        int gamesCount,
        IBackOfficeContext backOfficeContext)
    {
        return new MainPageBackOfficeContext(
            botsCount,
            usersCount,
            mailingsCount,
            gamesCount,
            backOfficeContext.UpdateContext,
            backOfficeContext.TelegramUser,
            backOfficeContext.EntryBot,
            backOfficeContext.Client,
            backOfficeContext.QrData,
            backOfficeContext.Identity);
    }

    private record MainPageBackOfficeContext(
        int BotsCount,
        int UsersCount,
        int MailingsCount,
        int GamesCount,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IMainPageBackOfficeContext;
}