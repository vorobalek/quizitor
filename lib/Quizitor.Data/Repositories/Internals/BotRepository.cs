using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Repositories.Internals;

internal sealed class BotRepository(ApplicationDbContext dbContext) : IBotRepository
{
    public async Task AddAsync(Bot bot, CancellationToken cancellationToken)
    {
        await dbContext.Bots.AddAsync(bot, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Bot[]> GetByMailingProfileIdAsync(
        int mailingProfileId,
        CancellationToken cancellationToken,
        MailingFilterFlagType? flagType = null)
    {
        return dbContext
            .MailingFilterBots
            .Where(x =>
                x.MailingProfileId == mailingProfileId &&
                (!flagType.HasValue || x.FlagType == flagType.Value))
            .Select(x => x.Bot)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Bot?> GetByIdOrDefaultAsync(int? botId, CancellationToken cancellationToken)
    {
        return await dbContext
            .Bots
            .SingleOrDefaultAsync(
                x => x.Id == botId,
                cancellationToken);
    }

    public Task<Bot[]> GetActiveGameServersAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .Bots
            .Where(x => x.Type == BotType.GameServer && x.IsActive && x.Username != null)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Bot[]> GetActiveGameAdminsAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .Bots
            .Where(x => x.Type == BotType.GameAdmin && x.IsActive && x.Username != null)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Bot?> GetCandidateGameAdminOrDefaultAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .Bots
            .Where(x => x.Type == BotType.GameAdmin && x.IsActive && x.Username != null)
            .OrderBy(x => x.GameAdminUsers.Count)
            .ThenBy(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Bot?> GetCandidateGameServerOrDefaultAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .Bots
            .Where(x => x.Type == BotType.GameServer && x.IsActive && x.Username != null)
            .OrderBy(x => x.GameServerUsers.Count)
            .ThenBy(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Bot[]> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Bots
            .OrderBy(x => x.Name)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .Bots
            .CountAsync(cancellationToken);
    }

    public Task UpdateAsync(Bot bot, CancellationToken cancellationToken)
    {
        dbContext.Bots.Update(bot);
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Bot[]> GetAllAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .Bots
            .ToArrayAsync(cancellationToken);
    }

    public Task<Bot[]> GetActiveAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .Bots
            .Where(x =>
                x.IsActive == true)
            .ToArrayAsync(cancellationToken);
    }
}