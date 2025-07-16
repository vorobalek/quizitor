using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Repositories;

public interface ISessionRepository
{
    Task AddAsync(Session session, CancellationToken cancellationToken);
    Task UpdateAsync(Session session, CancellationToken cancellationToken);
    Task<Session> GetByIdAsync(int sessionId, CancellationToken cancellationToken);
    Task<Session?> GetByIdOrDefaultAsync(int? sessionId, CancellationToken cancellationToken);
    Task<Session[]> GetAllNoTrackingAsync(CancellationToken cancellationToken);
    Task<int> CountByGameIdAsync(int gameId, CancellationToken cancellationToken);
    Task<int> CountByUserIdAsync(long userId, CancellationToken cancellationToken);
    Task<Session[]> GetPaginatedByGameIdAsync(int gameId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<Session[]> GetPaginatedAfterCreationByGameIdAsync(int gameId, int pageSize, CancellationToken cancellationToken);

    Task<Session[]> GetByMailingProfileIdAsync(
        int mailingProfileId,
        CancellationToken cancellationToken,
        MailingFilterFlagType? flagType = null);

    Task<Session[]> GetPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
}