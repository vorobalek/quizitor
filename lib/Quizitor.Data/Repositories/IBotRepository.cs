using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Repositories;

public interface IBotRepository
{
    Task<Bot?> GetByIdOrDefaultAsync(int? botId, CancellationToken cancellationToken);
    Task<Bot[]> GetActiveGameServersAsync(CancellationToken cancellationToken);
    Task<Bot[]> GetActiveGameAdminsAsync(CancellationToken cancellationToken);
    Task<Bot?> GetCandidateGameAdminOrDefaultAsync(CancellationToken cancellationToken);
    Task<Bot?> GetCandidateGameServerOrDefaultAsync(CancellationToken cancellationToken);

    Task<Bot[]> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);
    Task UpdateAsync(Bot bot, CancellationToken cancellationToken);
    Task<Bot[]> GetAllAsync(CancellationToken cancellationToken);
    Task<Bot[]> GetActiveAsync(CancellationToken cancellationToken);
    Task AddAsync(Bot bot, CancellationToken cancellationToken);

    Task<Bot[]> GetByMailingProfileIdAsync(
        int mailingProfileId,
        CancellationToken cancellationToken,
        MailingFilterFlagType? flagType = null);
}