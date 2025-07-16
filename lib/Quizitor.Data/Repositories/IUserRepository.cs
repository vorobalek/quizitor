using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task UpdateAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetByIdOrDefaultAsync(long userId, CancellationToken cancellationToken);

    Task<User[]> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<User[]> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken);

    Task<string[]> GetAllPermissionsAsync(long userId, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);

    Task<int> CountByGameServerIdAsync(int gameServerId, CancellationToken cancellationToken);

    Task<int> CountByGameAdminIdAsync(int gameAdminId, CancellationToken cancellationToken);
    Task<int> CountBySessionIdAsync(int sessionId, CancellationToken cancellationToken);
    Task<int> CountByGameIdAsync(int gameId, CancellationToken cancellationToken);
    Task<User[]> GetAllAsync(CancellationToken cancellationToken);
    Task<IGrouping<long, int>[]> GetForMailingBroadcastAsync(CancellationToken cancellationToken);
    Task<User[]> GetGameAdminsBySessionIdAsync(int sessionId, long[] authorizedUserIds, CancellationToken cancellationToken);
    Task<UserPermission[]> GetPermissionsByUserIdAsync(long userId, CancellationToken cancellationToken);

    Task<UserPrompt?> GetPromptByUserIdBotIdOrDefaultAsync(
        long userId,
        int? botId,
        CancellationToken cancellationToken);

    Task SetPromptByUserIdBotIdAsync(
        long userId,
        int? botId,
        UserPromptType type,
        string subject,
        CancellationToken cancellationToken);

    Task RemovePromptByUserIdBotIdAsync(
        long userId,
        int? botId,
        CancellationToken cancellationToken);

    Task RemovePromptByUserIdBotIdIfExistsAsync(
        long userId,
        int? botId,
        UserPromptType type,
        CancellationToken cancellationToken);

    Task<User?> GetLeaderByTeamIdSessionIdOrDefaultAsync(int teamId, int sessionId, CancellationToken cancellationToken);
    Task<User[]> GetMembersByTeamIdSessionIdAsync(int teamId, int sessionId, CancellationToken cancellationToken);

    Task<User[]> GetByMailingProfileIdAsync(
        int mailingProfileId,
        CancellationToken cancellationToken,
        MailingFilterFlagType? flagType = null);
}