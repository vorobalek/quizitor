using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;

internal interface IMailingPreviewSchemaBackofficeContext : IMailingProfileBackOfficeContext
{
    int SchemaPageNumber { get; }
    int SchemaPageCount { get; }

    static IMailingPreviewSchemaBackofficeContext Create(
        int schemaPageNumber,
        int schemaPageCount,
        IMailingProfileBackOfficeContext baseContext)
    {
        return new MailingPreviewSchemaBackofficeContext(
            schemaPageNumber,
            schemaPageCount,
            baseContext.Mailing,
            baseContext.MailingPageNumber,
            baseContext.IncludedGames,
            baseContext.ExcludedGames,
            baseContext.IncludedSessions,
            baseContext.ExcludedSessions,
            baseContext.IncludedTeams,
            baseContext.ExcludedTeams,
            baseContext.IncludedUsers,
            baseContext.ExcludedUsers,
            baseContext.IncludedBots,
            baseContext.ExcludedBots,
            baseContext.IncludedBotTypes,
            baseContext.ExcludedBotTypes,
            baseContext.ContactType,
            baseContext.PredictedUsersCount,
            baseContext.PredictedMessagesCount,
            baseContext.PredictedBotsCount,
            baseContext.Schema,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record MailingPreviewSchemaBackofficeContext(
        int SchemaPageNumber,
        int SchemaPageCount,
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
        IIdentity Identity) : IMailingPreviewSchemaBackofficeContext;
}