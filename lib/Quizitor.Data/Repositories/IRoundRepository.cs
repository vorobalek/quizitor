using Quizitor.Data.Entities;

namespace Quizitor.Data.Repositories;

public interface IRoundRepository
{
    Task AddAsync(Round round, CancellationToken cancellationToken);
    Task<Round> GetByIdAsync(int roundId, CancellationToken cancellationToken);
    Task<Round?> GetByIdOrDefaultAsync(int roundId, CancellationToken cancellationToken);
    Task RemoveRangeAsync(IEnumerable<Round> rounds, CancellationToken cancellationToken);
    Task<Round[]> GetByGameIdAsync(int gameId, CancellationToken cancellationToken);

    Task<Round[]> GetPaginatedByGameIdAsync(
        int gameId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<int> CountByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken);

    Task<int> GetNextNumberByGameIdAsync(int gameId, CancellationToken cancellationToken);
    Task<Round[]> GetPaginatedAfterCreationByGameIdAsync(int gameId, int pageSize, CancellationToken cancellationToken);
}