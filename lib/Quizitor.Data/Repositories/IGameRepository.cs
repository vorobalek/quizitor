using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Repositories;

public interface IGameRepository
{
    Task AddAsync(Game game, CancellationToken cancellationToken);
    Task UpdateAsync(Game game, CancellationToken cancellationToken);
    Task RemoveAsync(Game game, CancellationToken cancellationToken);
    Task<Game> GetByIdAsync(int gameId, CancellationToken cancellationToken);
    Task<Game?> GetByIdOrDefaultAsync(int gameId, CancellationToken cancellationToken);
    Task<Game[]> GetPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
    Task<Game[]> GetPaginatedAfterDeletionAsync(int gameId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<Game[]> GetPaginatedAfterCreationAsync(int pageSize, CancellationToken cancellationToken);

    Task<Game[]> GetByMailingProfileIdAsync(
        int mailingProfileId,
        CancellationToken cancellationToken,
        MailingFilterFlagType? flagType = null);
}