using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Infrastructure;

internal interface IMailingFilterBackOfficeContext<TEntity> : IMailingFilterListBackOfficeContext<TEntity>
{
    TEntity Entity { get; }

    static IMailingFilterBackOfficeContext<TEntity> Create(
        TEntity entity,
        IMailingFilterListBackOfficeContext<TEntity> baseContext)
    {
        return new MailingFilterBackOfficeContext(
            entity,
            baseContext.Entities,
            baseContext.PageNumber,
            baseContext.PageCount,
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

    private record MailingFilterBackOfficeContext(
        TEntity Entity,
        TEntity[] Entities,
        int PageNumber,
        int PageCount,
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
        IIdentity Identity) : IMailingFilterBackOfficeContext<TEntity>;
}