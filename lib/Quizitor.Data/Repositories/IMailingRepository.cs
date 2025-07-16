using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Repositories;

public interface IMailingRepository
{
    Task AddAsync(Mailing mailing, CancellationToken cancellationToken);

    Task<Mailing[]> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);
    Task<Mailing> GetByIdAsync(int mailingId, CancellationToken cancellationToken);
    Task<Mailing?> GetByIdOrDefaultAsync(int mailingId, CancellationToken cancellationToken);
    Task<Mailing[]> GetPaginatedAfterCreationAsync(int pageSize, CancellationToken cancellationToken);
    Task<MailingProfile?> GetProfileByIdOrDefaultAsync(int mailingProfileId, CancellationToken cancellationToken);

    Task<User[]> GetRecipientsAsync(
        int[] includedGameIds,
        int[] excludedGameIds,
        int[] includedSessionIds,
        int[] excludedSessionIds,
        int[] includedTeamIds,
        int[] excludedTeamIds,
        long[] includedUserIds,
        long[] excludedUserIds,
        CancellationToken cancellationToken);

    Task<IGrouping<User, Bot>[]> GetMailingSchemaForUsersAsync(
        MailingProfileContactType contactType,
        BotType[] includedBotTypes,
        BotType[] excludedBotTypes,
        int[] includedBotIds,
        int[] excludedBotIds,
        long[] userIds,
        CancellationToken cancellationToken);

    Task<MailingProfile?> GetProfileByMailingIdUserIdOrDefaultAsync(int mailingId, long userId, CancellationToken cancellationToken);
    Task ExcludeGameMailingFilterAsync(int mailingId, long ownerId, int gameId, CancellationToken cancellationToken);
    Task IncludeGameMailingFilterAsync(int mailingId, long ownerId, int gameId, CancellationToken cancellationToken);
    Task RemoveGameMailingFilterAsync(int mailingId, long ownerId, int gameId, CancellationToken cancellationToken);
    Task ExcludeSessionMailingFilterAsync(int mailingId, long ownerId, int sessionId, CancellationToken cancellationToken);
    Task RemoveSessionMailingFilterAsync(int mailingId, long ownerId, int sessionId, CancellationToken cancellationToken);
    Task IncludeSessionMailingFilterAsync(int mailingId, long ownerId, int sessionId, CancellationToken cancellationToken);
    Task ExcludeTeamMailingFilterAsync(int mailingId, long ownerId, int teamId, CancellationToken cancellationToken);
    Task RemoveTeamMailingFilterAsync(int mailingId, long ownerId, int teamId, CancellationToken cancellationToken);
    Task IncludeTeamMailingFilterAsync(int mailingId, long ownerId, int teamId, CancellationToken cancellationToken);
    Task ExcludeUserMailingFilterAsync(int mailingId, long ownerId, long userId, CancellationToken cancellationToken);
    Task RemoveUserMailingFilterAsync(int mailingId, long ownerId, long userId, CancellationToken cancellationToken);
    Task IncludeUserMailingFilterAsync(int mailingId, long ownerId, long userId, CancellationToken cancellationToken);
    Task ExcludeBotMailingFilterAsync(int mailingId, long ownerId, int botId, CancellationToken cancellationToken);
    Task RemoveBotMailingFilterAsync(int mailingId, long ownerId, int botId, CancellationToken cancellationToken);
    Task IncludeBotMailingFilterAsync(int mailingId, long ownerId, int botId, CancellationToken cancellationToken);
    Task ExcludeBotTypeMailingFilterAsync(int mailingId, long ownerId, BotType botType, CancellationToken cancellationToken);
    Task RemoveBotTypeMailingFilterAsync(int mailingId, long ownerId, BotType botType, CancellationToken cancellationToken);
    Task IncludeBotTypeMailingFilterAsync(int mailingId, long ownerId, BotType botType, CancellationToken cancellationToken);
    Task SetMailingProfileContactTypeAsync(int mailingId, long ownerId, MailingProfileContactType contactType, CancellationToken cancellationToken);
}