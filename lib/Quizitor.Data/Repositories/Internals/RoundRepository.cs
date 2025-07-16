using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;

namespace Quizitor.Data.Repositories.Internals;

internal sealed class RoundRepository(
    ApplicationDbContext dbContext) : IRoundRepository
{
    public async Task AddAsync(Round round, CancellationToken cancellationToken)
    {
        await dbContext.Rounds.AddAsync(round, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Round> GetByIdAsync(int roundId, CancellationToken cancellationToken)
    {
        return await GetByIdOrDefaultAsync(
                   roundId,
                   cancellationToken)
               ?? throw new InvalidOperationException(
                   string.Format(
                       Constants.NoRoundWithIdFound,
                       roundId));
    }

    public async Task<Round?> GetByIdOrDefaultAsync(int roundId, CancellationToken cancellationToken)
    {
        return await dbContext
            .Rounds
            .SingleOrDefaultAsync(
                x => x.Id == roundId,
                cancellationToken);
    }

    public Task RemoveRangeAsync(IEnumerable<Round> rounds, CancellationToken cancellationToken)
    {
        dbContext.Rounds.RemoveRange(rounds);
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Round[]> GetByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Rounds
            .Where(r => r.GameId == gameId)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Round[]> GetPaginatedByGameIdAsync(
        int gameId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Rounds
            .Where(x => x.GameId == gameId)
            .OrderBy(x => x.Number)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> CountByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Rounds
            .Where(x => x.GameId == gameId)
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetNextNumberByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken)
    {
        return ((await dbContext
            .Rounds
            .Where(x => x.GameId == gameId)
            .OrderByDescending(x => x.Number)
            .FirstOrDefaultAsync(cancellationToken))?.Number ?? 0) + 1;
    }

    public Task<Round[]> GetPaginatedAfterCreationByGameIdAsync(
        int gameId,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Rounds
            .Where(x => x.GameId == gameId)
            .Take(pageSize - 1)
            .ToArrayAsync(cancellationToken);
    }
}