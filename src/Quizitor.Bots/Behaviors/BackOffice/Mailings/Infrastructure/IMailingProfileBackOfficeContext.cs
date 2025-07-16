using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;

internal interface IMailingProfileBackOfficeContext : IBackOfficeContext
{
    Mailing Mailing { get; }
    int MailingPageNumber { get; }
    Game[] IncludedGames { get; }
    Game[] ExcludedGames { get; }
    Session[] IncludedSessions { get; }
    Session[] ExcludedSessions { get; }
    Team[] IncludedTeams { get; }
    Team[] ExcludedTeams { get; }
    User[] IncludedUsers { get; }
    User[] ExcludedUsers { get; }
    Bot[] IncludedBots { get; }
    Bot[] ExcludedBots { get; }
    BotType[] IncludedBotTypes { get; }
    BotType[] ExcludedBotTypes { get; }
    MailingProfileContactType ContactType { get; }
    int PredictedUsersCount { get; }
    int PredictedMessagesCount { get; }
    int PredictedBotsCount { get; }
    IGrouping<User, Bot>[] Schema { get; }

    static IMailingProfileBackOfficeContext Create(
        Mailing mailing,
        int mailingPageNumber,
        Game[] includedGames,
        Game[] excludedGames,
        Session[] includedSessions,
        Session[] excludedSessions,
        Team[] includedTeams,
        Team[] excludedTeams,
        User[] includedUsers,
        User[] excludedUsers,
        Bot[] includedBots,
        Bot[] excludedBots,
        BotType[] includedBotTypes,
        BotType[] excludedBotTypes,
        MailingProfileContactType contactType,
        int predictedUsersCount,
        int predictedMessagesCount,
        int predictedBotsCount,
        IGrouping<User, Bot>[] schema,
        IBackOfficeContext baseContext)
    {
        return new MailingProfileBackOfficeContext(
            mailing,
            mailingPageNumber,
            includedGames,
            excludedGames,
            includedSessions,
            excludedSessions,
            includedTeams,
            excludedTeams,
            includedUsers,
            excludedUsers,
            includedBots,
            excludedBots,
            includedBotTypes,
            excludedBotTypes,
            contactType,
            predictedUsersCount,
            predictedMessagesCount,
            predictedBotsCount,
            schema,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record MailingProfileBackOfficeContext(
        Mailing Mailing,
        int MailingPageNumber,
        Game[] IncludedGames,
        Game[] ExcludedGames,
        Session[] IncludedSessions,
        Session[] ExcludedSessions,
        Team[] IncludedTeams,
        Team[] ExcludedTeams,
        User[] IncludedUsers,
        User[] ExcludedUsers,
        Bot[] IncludedBots,
        Bot[] ExcludedBots,
        BotType[] IncludedBotTypes,
        BotType[] ExcludedBotTypes,
        MailingProfileContactType ContactType,
        int PredictedUsersCount,
        int PredictedMessagesCount,
        int PredictedBotsCount,
        IGrouping<User, Bot>[] Schema,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IMailingProfileBackOfficeContext;
}