using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Repositories.Internals;

internal sealed class GameRepository(
    ApplicationDbContext dbContext) : IGameRepository
{
    public async Task AddAsync(Game game, CancellationToken cancellationToken)
    {
        await dbContext.Games.AddAsync(game, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(Game game, CancellationToken cancellationToken)
    {
        dbContext.Games.Update(game);
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task RemoveAsync(
        Game game,
        CancellationToken cancellationToken)
    {
        dbContext.Games.Remove(game);
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Game> GetByIdAsync(
        int gameId,
        CancellationToken cancellationToken)
    {
        return await GetByIdOrDefaultAsync(
                   gameId,
                   cancellationToken)
               ?? throw new InvalidOperationException(
                   string.Format(
                       Constants.NoGameWithIdFound,
                       gameId));
    }

    public async Task<Game?> GetByIdOrDefaultAsync(
        int gameId,
        CancellationToken cancellationToken)
    {
        return await dbContext
            .Games
            .SingleOrDefaultAsync(
                x => x.Id == gameId,
                cancellationToken);
    }

    public Task<Game[]> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Games
            .OrderByDescending(x => x.Id)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .Games
            .CountAsync(cancellationToken);
    }

    public Task<Game[]> GetPaginatedAfterDeletionAsync(
        int gameId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Games
            .Where(x => x.Id != gameId)
            .OrderByDescending(x => x.Id)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Game[]> GetPaginatedAfterCreationAsync(
        int pageSize,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Games
            .OrderByDescending(x => x.Id)
            .Take(pageSize - 1)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Game[]> GetByMailingProfileIdAsync(
        int mailingProfileId,
        CancellationToken cancellationToken,
        MailingFilterFlagType? flagType = null)
    {
        return dbContext
            .MailingFilterGames
            .Where(x =>
                x.MailingProfileId == mailingProfileId &&
                (!flagType.HasValue || x.FlagType == flagType.Value))
            .Select(x => x.Game)
            .ToArrayAsync(cancellationToken);
    }
}