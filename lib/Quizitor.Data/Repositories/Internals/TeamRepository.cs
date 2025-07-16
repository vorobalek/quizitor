using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Repositories.Internals;

internal sealed class TeamRepository(
    ApplicationDbContext dbContext) : ITeamRepository
{
    public async Task AddAsync(Team team, CancellationToken cancellationToken)
    {
        await dbContext.Teams.AddAsync(team, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Team?> GetByIdOrDefaultAsync(
        int teamId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Teams
            .SingleOrDefaultAsync(
                x => x.Id == teamId,
                cancellationToken);
    }

    public async Task<Team?> GetBySessionIdUserIdOrDefaultAsync(
        int sessionId,
        long userId,
        CancellationToken cancellationToken)
    {
        return await dbContext
            .TeamMembers
            .Where(x =>
                x.SessionId == sessionId &&
                x.UserId == userId)
            .Select(x => x.Team)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<Team[]> GetPaginatedByOwnerIdAsync(
        long ownerId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Teams
            .Where(x => x.OwnerId == ownerId)
            .OrderByDescending(x => x.Id)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> CountByOwnerIdAsync(
        long ownerId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Teams
            .Where(x => x.OwnerId == ownerId)
            .CountAsync(cancellationToken);
    }

    public async Task AddMemberAsync(
        long userId,
        int sessionId,
        int teamId,
        CancellationToken cancellationToken)
    {
        var teamMember = await dbContext
            .TeamMembers
            .SingleOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.SessionId == sessionId,
                cancellationToken);

        if (teamMember is null)
        {
            await dbContext
                .TeamMembers
                .AddAsync(
                    new TeamMember
                    {
                        TeamId = teamId,
                        SessionId = sessionId,
                        UserId = userId
                    },
                    cancellationToken);
        }
        else
        {
            teamMember.TeamId = teamId;
            dbContext.TeamMembers.Update(teamMember);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SetLeaderAsync(
        int teamId,
        int sessionId,
        long userId,
        CancellationToken cancellationToken)
    {
        var teamLeader = await dbContext
            .TeamLeaders
            .SingleOrDefaultAsync(x =>
                    x.TeamId == teamId &&
                    x.SessionId == sessionId,
                cancellationToken);

        if (teamLeader is null)
        {
            await dbContext
                .TeamLeaders
                .AddAsync(
                    new TeamLeader
                    {
                        TeamId = teamId,
                        SessionId = sessionId,
                        UserId = userId
                    },
                    cancellationToken);
        }
        else
        {
            teamLeader.UserId = userId;
            dbContext.TeamLeaders.Update(teamLeader);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UnsetLeaderAsync(
        int teamId,
        int sessionId,
        CancellationToken cancellationToken)
    {
        var teamLeader = await dbContext
            .TeamLeaders
            .SingleOrDefaultAsync(x =>
                    x.TeamId == teamId &&
                    x.SessionId == sessionId,
                cancellationToken);

        if (teamLeader != null)
        {
            dbContext.TeamLeaders.Remove(teamLeader);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task LeaveAsync(
        int teamId,
        int sessionId,
        long userId, CancellationToken cancellationToken)
    {
        var needSave = false;
        var teamMember = await dbContext
            .TeamMembers
            .SingleOrDefaultAsync(x =>
                    x.TeamId == teamId &&
                    x.SessionId == sessionId &&
                    x.UserId == userId,
                cancellationToken);

        if (teamMember != null)
        {
            dbContext.TeamMembers.Remove(teamMember);
            needSave = true;
        }

        var teamLeader = await dbContext
            .TeamLeaders
            .SingleOrDefaultAsync(x =>
                    x.TeamId == teamId &&
                    x.SessionId == sessionId &&
                    x.UserId == userId,
                cancellationToken);

        if (teamLeader != null)
        {
            dbContext.TeamLeaders.Remove(teamLeader);
            needSave = true;
        }

        if (needSave)
            await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Team[]> GetPaginatedByOwnerIdAfterCreationAsync(
        long ownerId,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Teams
            .Where(x => x.OwnerId == ownerId)
            .OrderByDescending(x => x.Id)
            .Take(pageSize - 1)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Team[]> GetByMailingProfileIdAsync(
        int mailingProfileId,
        CancellationToken cancellationToken,
        MailingFilterFlagType? flagType = null)
    {
        return dbContext
            .MailingFilterTeams
            .Where(x =>
                x.MailingProfileId == mailingProfileId &&
                (!flagType.HasValue || x.FlagType == flagType.Value))
            .Select(x => x.Team)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Team[]> GetPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return dbContext
            .Teams
            .OrderByDescending(x => x.Id)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .Teams
            .CountAsync(cancellationToken);
    }

    public Task<Team?> GetByNameOrDefaultAsync(string teamName, CancellationToken cancellationToken)
    {
        return dbContext
            .Teams
            .SingleOrDefaultAsync(x =>
                    x.Name == teamName,
                cancellationToken);
    }
}