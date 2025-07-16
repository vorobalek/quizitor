using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Repositories.Internals;

internal sealed class UserRepository(
    ApplicationDbContext dbContext) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        dbContext.Users.Update(user);
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetByIdOrDefaultAsync(long userId, CancellationToken cancellationToken)
    {
        return await dbContext
            .Users
            .SingleOrDefaultAsync(
                x => x.Id == userId,
                cancellationToken);
    }

    public Task<User[]> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Users
            .OrderBy(x => x.Id)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .Users
            .CountAsync(cancellationToken);
    }

    public Task<int> CountByGameServerIdAsync(int gameServerId, CancellationToken cancellationToken)
    {
        return dbContext
            .Users
            .CountAsync(
                x => x.GameServerId == gameServerId,
                cancellationToken);
    }

    public Task<int> CountByGameAdminIdAsync(int gameAdminId, CancellationToken cancellationToken)
    {
        return dbContext
            .Users
            .CountAsync(
                x => x.GameAdminId == gameAdminId,
                cancellationToken);
    }

    public Task<int> CountBySessionIdAsync(
        int sessionId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Users
            .CountAsync(
                x => x.SessionId == sessionId,
                cancellationToken);
    }

    public Task<int> CountByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Users
            .CountAsync(x =>
                    x.Session != null &&
                    x.Session.GameId == gameId,
                cancellationToken);
    }

    public Task<User[]> GetAllAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .Users
            .OrderBy(x => x.Id)
            .ToArrayAsync(cancellationToken);
    }

    public Task<IGrouping<long, int>[]> GetForMailingBroadcastAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .BotInteractions
            .Join(
                dbContext.Bots.Where(x => x.Username != null),
                x => x.BotUsername,
                x => x.Username,
                (interaction, bot) => new
                {
                    UserId = interaction.User.Id,
                    BotId = bot.Id
                })
            .GroupBy(x => x.UserId, x => x.BotId)
            .ToArrayAsync(cancellationToken);
    }

    public Task<User[]> GetGameAdminsBySessionIdAsync(
        int sessionId,
        long[] authorizedUserIds,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Users
            .Where(x =>
                x.SessionId == sessionId &&
                x.GameAdminId.HasValue &&
                x.Roles.Any(r => r.SystemName == Role.GameAdmin) ||
                authorizedUserIds.Contains(x.Id))
            .ToArrayAsync(cancellationToken);
    }

    public Task<User?> GetLeaderByTeamIdSessionIdOrDefaultAsync(
        int teamId,
        int sessionId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .TeamLeaders
            .Where(x =>
                x.TeamId == teamId &&
                x.SessionId == sessionId)
            .Select(x => x.User)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<User[]> GetMembersByTeamIdSessionIdAsync(
        int teamId,
        int sessionId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .TeamMembers
            .Where(x =>
                x.TeamId == teamId &&
                x.SessionId == sessionId)
            .Select(x => x.User)
            .OrderBy(x => x.Id)
            .ToArrayAsync(cancellationToken);
    }

    public Task<User[]> GetByMailingProfileIdAsync(
        int mailingProfileId,
        CancellationToken cancellationToken,
        MailingFilterFlagType? flagType = null)
    {
        return dbContext
            .MailingFilterUsers
            .Where(x =>
                mailingProfileId == x.MailingProfileId &&
                (!flagType.HasValue || x.FlagType == flagType.Value))
            .Select(x => x.User)
            .ToArrayAsync(cancellationToken);
    }

    public Task<User[]> GetBySessionIdAsync(
        int sessionId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Users
            .Where(x => x.SessionId == sessionId)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<string[]> GetAllPermissionsAsync(
        long userId,
        CancellationToken cancellationToken)
    {
        var userQuery = dbContext
            .Users
            .Where(x => x.Id == userId);

        var userPermissions = await userQuery
            .Include(x => x.Permissions)
            .SelectMany(x => x.Permissions.Select(p => p.SystemName))
            .ToArrayAsync(cancellationToken);

        var rolePermissions = await userQuery
            .Include(x => x.Roles)
            .ThenInclude(x => x.Permissions)
            .SelectMany(x => x.Roles.SelectMany(e => e.Permissions.Select(p => p.SystemName)))
            .ToArrayAsync(cancellationToken);

        return userPermissions.Concat(rolePermissions).Distinct().ToArray();
    }

    public Task<UserPermission[]> GetPermissionsByUserIdAsync(long userId, CancellationToken cancellationToken)
    {
        return dbContext
            .UserPermissions
            .Where(x => x.UserId == userId)
            .ToArrayAsync(cancellationToken);
    }

    public Task<UserPrompt?> GetPromptByUserIdBotIdOrDefaultAsync(
        long userId,
        int? botId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .UserPrompts
            .SingleOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.BotId == botId,
                cancellationToken);
    }

    public async Task SetPromptByUserIdBotIdAsync(
        long userId,
        int? botId,
        UserPromptType type,
        string subject,
        CancellationToken cancellationToken)
    {
        var prompt = await dbContext
            .UserPrompts
            .SingleOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.BotId == botId,
                cancellationToken);

        if (prompt is not null)
        {
            prompt.Type = type;
            prompt.Subject = subject;
            dbContext.UserPrompts.Update(prompt);
        }

        else
        {
            prompt = new UserPrompt
            {
                UserId = userId,
                BotId = botId,
                Type = type,
                Subject = subject
            };
            await dbContext.UserPrompts.AddAsync(prompt, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemovePromptByUserIdBotIdAsync(
        long userId,
        int? botId,
        CancellationToken cancellationToken)
    {
        var prompt = await dbContext
            .UserPrompts
            .SingleOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.BotId == botId,
                cancellationToken);

        if (prompt is not null)
        {
            dbContext.UserPrompts.Remove(prompt);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemovePromptByUserIdBotIdIfExistsAsync(
        long userId,
        int? botId,
        UserPromptType type,
        CancellationToken cancellationToken)
    {
        var prompt = await dbContext
            .UserPrompts
            .SingleOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.BotId == botId,
                cancellationToken);

        if (prompt?.Type == type)
        {
            dbContext.UserPrompts.Remove(prompt);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}