using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Repositories.Internals;

internal sealed class SessionRepository(
    ApplicationDbContext dbContext) : ISessionRepository
{
    public async Task AddAsync(
        Session session,
        CancellationToken cancellationToken)
    {
        await dbContext.Sessions.AddAsync(session, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(
        Session session,
        CancellationToken cancellationToken)
    {
        dbContext.Sessions.Update(session);
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Session> GetByIdAsync(
        int sessionId,
        CancellationToken cancellationToken)
    {
        return await GetByIdOrDefaultAsync(
                   sessionId,
                   cancellationToken)
               ?? throw new InvalidOperationException(
                   string
                       .Format(
                           Constants.NoSessionWithIdFound,
                           sessionId));
    }

    public async Task<Session?> GetByIdOrDefaultAsync(
        int? sessionId,
        CancellationToken cancellationToken)
    {
        return await dbContext
            .Sessions
            .SingleOrDefaultAsync(
                x => x.Id == sessionId,
                cancellationToken);
    }

    public Task<Session[]> GetAllNoTrackingAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .Sessions
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> CountByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Sessions
            .Where(x => x.GameId == gameId)
            .CountAsync(cancellationToken);
    }

    public Task<int> CountByUserIdAsync(long userId, CancellationToken cancellationToken)
    {
        return dbContext
            .Sessions
            .Where(x => x.Submissions.Any(e => e.UserId == userId))
            .CountAsync(cancellationToken);
    }

    public Task<Session[]> GetPaginatedByGameIdAsync(
        int gameId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Sessions
            .Where(x => x.GameId == gameId)
            .OrderByDescending(x => x.Id)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Session[]> GetPaginatedAfterCreationByGameIdAsync(
        int gameId,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Sessions
            .Where(x => x.GameId == gameId)
            .OrderByDescending(x => x.Id)
            .Take(pageSize - 1)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Session[]> GetByMailingProfileIdAsync(
        int mailingProfileId,
        CancellationToken cancellationToken,
        MailingFilterFlagType? flagType = null)
    {
        return dbContext
            .MailingFilterSessions
            .Include(x => x.Session)
            .ThenInclude(x => x.Game)
            .Where(x =>
                x.MailingProfileId == mailingProfileId &&
                (!flagType.HasValue || x.FlagType == flagType.Value))
            .Select(x => x.Session)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Session[]> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Sessions
            .Include(x => x.Game)
            .OrderByDescending(x => x.Id)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .Sessions
            .CountAsync(cancellationToken);
    }
}