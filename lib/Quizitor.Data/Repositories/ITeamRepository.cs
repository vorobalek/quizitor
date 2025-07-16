using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Repositories;

public interface ITeamRepository
{
    Task AddAsync(Team team, CancellationToken cancellationToken);
    Task<Team?> GetByIdOrDefaultAsync(int teamId, CancellationToken cancellationToken);

    Task<Team?> GetBySessionIdUserIdOrDefaultAsync(
        int sessionId,
        long userId,
        CancellationToken cancellationToken);

    Task<Team[]> GetPaginatedByOwnerIdAsync(long ownerId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<int> CountByOwnerIdAsync(long ownerId, CancellationToken cancellationToken);
    Task AddMemberAsync(long userId, int sessionId, int teamId, CancellationToken cancellationToken);
    Task SetLeaderAsync(int teamId, int sessionId, long userId, CancellationToken cancellationToken);
    Task UnsetLeaderAsync(int teamId, int sessionId, CancellationToken cancellationToken);
    Task LeaveAsync(int teamId, int sessionId, long userId, CancellationToken cancellationToken);
    Task<Team[]> GetPaginatedByOwnerIdAfterCreationAsync(long ownerId, int pageSize, CancellationToken cancellationToken);

    Task<Team[]> GetByMailingProfileIdAsync(
        int mailingProfileId,
        CancellationToken cancellationToken,
        MailingFilterFlagType? flagType = null);

    Task<Team[]> GetPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
    Task<Team?> GetByNameOrDefaultAsync(string teamName, CancellationToken cancellationToken);
}