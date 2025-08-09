using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Repositories.Internals;

internal sealed class MailingRepository(
    ApplicationDbContext dbContext) : IMailingRepository
{
    public async Task AddAsync(Mailing mailing, CancellationToken cancellationToken)
    {
        await dbContext.Mailings.AddAsync(mailing, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Mailing[]> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Mailings
            .OrderByDescending(x => x.Id)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .Mailings
            .CountAsync(cancellationToken);
    }

    public async Task<Mailing> GetByIdAsync(
        int mailingId,
        CancellationToken cancellationToken)
    {
        return await GetByIdOrDefaultAsync(
                   mailingId,
                   cancellationToken)
               ?? throw new InvalidOperationException(
                   string.Format(
                       Constants.NoMailingWithIdFound,
                       mailingId));
    }

    public async Task<Mailing?> GetByIdOrDefaultAsync(
        int mailingId,
        CancellationToken cancellationToken)
    {
        return await dbContext
            .Mailings
            .SingleOrDefaultAsync(
                x => x.Id == mailingId,
                cancellationToken);
    }

    public Task<Mailing[]> GetPaginatedAfterCreationAsync(
        int pageSize,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Mailings
            .OrderByDescending(x => x.Id)
            .Take(pageSize - 1)
            .ToArrayAsync(cancellationToken);
    }

    public Task<Mailing[]> GetPaginatedAfterDeletionAsync(int mailingId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return dbContext
            .Mailings
            .Where(x => x.Id != mailingId)
            .OrderByDescending(x => x.Id)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public Task<User[]> GetRecipientsAsync(
        int[] includedGameIds,
        int[] excludedGameIds,
        int[] includedSessionIds,
        int[] excludedSessionIds,
        int[] includedTeamIds,
        int[] excludedTeamIds,
        long[] includedUserIds,
        long[] excludedUserIds,
        CancellationToken cancellationToken)
    {
        var usersQuery = GetRecipientsQuery(
            includedGameIds,
            excludedGameIds,
            includedSessionIds,
            excludedSessionIds,
            includedTeamIds,
            excludedTeamIds,
            includedUserIds,
            excludedUserIds);

        return usersQuery
            .ToArrayAsync(cancellationToken);
    }

    public Task<IGrouping<User, Bot>[]> GetMailingSchemaForUsersAsync(
        MailingProfileContactType contactType,
        BotType[] includedBotTypes,
        BotType[] excludedBotTypes,
        int[] includedBotIds,
        int[] excludedBotIds,
        long[] userIds,
        CancellationToken cancellationToken)
    {
        var botsQuery = dbContext
            .Bots
            .Where(x => x.Username != null)
            .AsQueryable();

        if (includedBotTypes.Length != 0)
        {
            botsQuery = botsQuery.Where(x => includedBotTypes.Contains(x.Type));
        }

        if (excludedBotTypes.Length != 0)
        {
            botsQuery = botsQuery.Where(x => !excludedBotTypes.Contains(x.Type));
        }

        if (includedBotIds.Length != 0)
        {
            botsQuery = botsQuery.Where(x => includedBotIds.Contains(x.Id));
        }

        if (excludedBotIds.Length != 0)
        {
            botsQuery = botsQuery.Where(x => !excludedBotIds.Contains(x.Id));
        }

        var baseQuery = dbContext
            .BotInteractions
            .Where(x => userIds.Contains(x.UserId))
            .Join(
                botsQuery,
                interaction => interaction.BotUsername,
                bot => bot.Username,
                (interaction, bot) => new
                {
                    interaction.User,
                    Bot = bot,
                    interaction.LastInteraction
                });

        return contactType switch
        {
            MailingProfileContactType.All => baseQuery
                .GroupBy(x => x.User, x => x.Bot)
                .ToArrayAsync(cancellationToken),
            MailingProfileContactType.LastInteraction => baseQuery
                .GroupBy(x => x.User)
                .Select(x => x
                    .OrderByDescending(e => e.LastInteraction)
                    .First()
                )
                .ToArrayAsync(cancellationToken)
                .ContinueWith(x => x
                        .Result
                        .GroupBy(
                            e => e.User,
                            e => e.Bot)
                        .ToArray(),
                    cancellationToken),
            _ => Task.FromResult<IGrouping<User, Bot>[]>([])
        };
    }

    public Task<MailingProfile?> GetProfileByMailingIdUserIdOrDefaultAsync(int mailingId, long userId, CancellationToken cancellationToken)
    {
        return dbContext
            .MailingProfiles
            .SingleOrDefaultAsync(x =>
                    x.MailingId == mailingId &&
                    x.OwnerId == userId,
                cancellationToken);
    }

    public Task ExcludeGameMailingFilterAsync(int mailingId, long ownerId, int gameId, CancellationToken cancellationToken)
    {
        return SetGameMailingFilter(mailingId, ownerId, gameId, MailingFilterFlagType.Exclude, cancellationToken);
    }

    public Task IncludeGameMailingFilterAsync(int mailingId, long ownerId, int gameId, CancellationToken cancellationToken)
    {
        return SetGameMailingFilter(mailingId, ownerId, gameId, MailingFilterFlagType.Include, cancellationToken);
    }

    public Task RemoveGameMailingFilterAsync(int mailingId, long ownerId, int gameId, CancellationToken cancellationToken)
    {
        return SetGameMailingFilter(mailingId, ownerId, gameId, MailingFilterFlagType.None, cancellationToken);
    }

    public Task ExcludeSessionMailingFilterAsync(int mailingId, long ownerId, int sessionId, CancellationToken cancellationToken)
    {
        return SetSessionMailingFilter(mailingId, ownerId, sessionId, MailingFilterFlagType.Exclude, cancellationToken);
    }

    public Task IncludeSessionMailingFilterAsync(int mailingId, long ownerId, int sessionId, CancellationToken cancellationToken)
    {
        return SetSessionMailingFilter(mailingId, ownerId, sessionId, MailingFilterFlagType.Include, cancellationToken);
    }

    public Task RemoveSessionMailingFilterAsync(int mailingId, long ownerId, int sessionId, CancellationToken cancellationToken)
    {
        return SetSessionMailingFilter(mailingId, ownerId, sessionId, MailingFilterFlagType.None, cancellationToken);
    }

    public Task ExcludeTeamMailingFilterAsync(int mailingId, long ownerId, int teamId, CancellationToken cancellationToken)
    {
        return SetTeamMailingFilter(mailingId, ownerId, teamId, MailingFilterFlagType.Exclude, cancellationToken);
    }

    public Task IncludeTeamMailingFilterAsync(int mailingId, long ownerId, int teamId, CancellationToken cancellationToken)
    {
        return SetTeamMailingFilter(mailingId, ownerId, teamId, MailingFilterFlagType.Include, cancellationToken);
    }

    public Task RemoveTeamMailingFilterAsync(int mailingId, long ownerId, int teamId, CancellationToken cancellationToken)
    {
        return SetTeamMailingFilter(mailingId, ownerId, teamId, MailingFilterFlagType.None, cancellationToken);
    }

    public Task ExcludeUserMailingFilterAsync(int mailingId, long ownerId, long userId, CancellationToken cancellationToken)
    {
        return SetUserMailingFilter(mailingId, ownerId, userId, MailingFilterFlagType.Exclude, cancellationToken);
    }

    public Task IncludeUserMailingFilterAsync(int mailingId, long ownerId, long userId, CancellationToken cancellationToken)
    {
        return SetUserMailingFilter(mailingId, ownerId, userId, MailingFilterFlagType.Include, cancellationToken);
    }

    public Task RemoveUserMailingFilterAsync(int mailingId, long ownerId, long userId, CancellationToken cancellationToken)
    {
        return SetUserMailingFilter(mailingId, ownerId, userId, MailingFilterFlagType.None, cancellationToken);
    }

    public async Task SetMailingProfileContactTypeAsync(int mailingId, long ownerId, MailingProfileContactType contactType, CancellationToken cancellationToken)
    {
        var mailingProfile = await GetProfileByMailingIdUserIdOrDefaultAsync(mailingId, ownerId, cancellationToken);
        if (mailingProfile is null)
        {
            mailingProfile = new MailingProfile
            {
                MailingId = mailingId,
                OwnerId = ownerId,
                ContactType = contactType
            };
            await dbContext.MailingProfiles.AddAsync(mailingProfile, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        mailingProfile.ContactType = contactType;
        dbContext.MailingProfiles.Update(mailingProfile);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task RemoveAsync(
        Mailing mailing,
        CancellationToken cancellationToken)
    {
        dbContext.Mailings.Remove(mailing);
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task ExcludeBotTypeMailingFilterAsync(int mailingId, long ownerId, BotType botType, CancellationToken cancellationToken)
    {
        return SetBotTypeMailingFilter(mailingId, ownerId, botType, MailingFilterFlagType.Exclude, cancellationToken);
    }

    public Task IncludeBotTypeMailingFilterAsync(int mailingId, long ownerId, BotType botType, CancellationToken cancellationToken)
    {
        return SetBotTypeMailingFilter(mailingId, ownerId, botType, MailingFilterFlagType.Include, cancellationToken);
    }

    public Task RemoveBotTypeMailingFilterAsync(int mailingId, long ownerId, BotType botType, CancellationToken cancellationToken)
    {
        return SetBotTypeMailingFilter(mailingId, ownerId, botType, MailingFilterFlagType.None, cancellationToken);
    }

    public Task ExcludeBotMailingFilterAsync(int mailingId, long ownerId, int botId, CancellationToken cancellationToken)
    {
        return SetBotMailingFilter(mailingId, ownerId, botId, MailingFilterFlagType.Exclude, cancellationToken);
    }

    public Task IncludeBotMailingFilterAsync(int mailingId, long ownerId, int botId, CancellationToken cancellationToken)
    {
        return SetBotMailingFilter(mailingId, ownerId, botId, MailingFilterFlagType.Include, cancellationToken);
    }

    public Task RemoveBotMailingFilterAsync(int mailingId, long ownerId, int botId, CancellationToken cancellationToken)
    {
        return SetBotMailingFilter(mailingId, ownerId, botId, MailingFilterFlagType.None, cancellationToken);
    }

    private async Task SetGameMailingFilter(
        int mailingId,
        long ownerId,
        int gameId,
        MailingFilterFlagType flagType,
        CancellationToken cancellationToken)
    {
        var mailingProfile = await GetProfileByMailingIdUserIdOrDefaultAsync(mailingId, ownerId, cancellationToken);
        if (mailingProfile is null)
        {
            mailingProfile = new MailingProfile
            {
                MailingId = mailingId,
                OwnerId = ownerId,
                Games =
                [
                    new MailingFilterGame
                    {
                        GameId = gameId,
                        FlagType = flagType
                    }
                ]
            };
            await dbContext.MailingProfiles.AddAsync(mailingProfile, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var filter = await dbContext
            .MailingFilterGames
            .SingleOrDefaultAsync(x =>
                    x.MailingProfileId == mailingProfile.Id &&
                    x.GameId == gameId,
                cancellationToken);
        if (filter is null)
        {
            filter = new MailingFilterGame
            {
                MailingProfileId = mailingProfile.Id,
                GameId = gameId,
                FlagType = flagType
            };
            await dbContext.MailingFilterGames.AddAsync(filter, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        filter.FlagType = flagType;
        dbContext.MailingFilterGames.Update(filter);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SetSessionMailingFilter(
        int mailingId,
        long ownerId,
        int sessionId,
        MailingFilterFlagType flagType,
        CancellationToken cancellationToken)
    {
        var mailingProfile = await GetProfileByMailingIdUserIdOrDefaultAsync(mailingId, ownerId, cancellationToken);
        if (mailingProfile is null)
        {
            mailingProfile = new MailingProfile
            {
                MailingId = mailingId,
                OwnerId = ownerId,
                Sessions =
                [
                    new MailingFilterSession
                    {
                        SessionId = sessionId,
                        FlagType = flagType
                    }
                ]
            };
            await dbContext.MailingProfiles.AddAsync(mailingProfile, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var filter = await dbContext
            .MailingFilterSessions
            .SingleOrDefaultAsync(x =>
                    x.MailingProfileId == mailingProfile.Id &&
                    x.SessionId == sessionId,
                cancellationToken);
        if (filter is null)
        {
            filter = new MailingFilterSession
            {
                MailingProfileId = mailingProfile.Id,
                SessionId = sessionId,
                FlagType = flagType
            };
            await dbContext.MailingFilterSessions.AddAsync(filter, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        filter.FlagType = flagType;
        dbContext.MailingFilterSessions.Update(filter);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SetTeamMailingFilter(
        int mailingId,
        long ownerId,
        int teamId,
        MailingFilterFlagType flagType,
        CancellationToken cancellationToken)
    {
        var mailingProfile = await GetProfileByMailingIdUserIdOrDefaultAsync(mailingId, ownerId, cancellationToken);
        if (mailingProfile is null)
        {
            mailingProfile = new MailingProfile
            {
                MailingId = mailingId,
                OwnerId = ownerId,
                Teams =
                [
                    new MailingFilterTeam
                    {
                        TeamId = teamId,
                        FlagType = flagType
                    }
                ]
            };
            await dbContext.MailingProfiles.AddAsync(mailingProfile, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var filter = await dbContext
            .MailingFilterTeams
            .SingleOrDefaultAsync(x =>
                    x.MailingProfileId == mailingProfile.Id &&
                    x.TeamId == teamId,
                cancellationToken);
        if (filter is null)
        {
            filter = new MailingFilterTeam
            {
                MailingProfileId = mailingProfile.Id,
                TeamId = teamId,
                FlagType = flagType
            };
            await dbContext.MailingFilterTeams.AddAsync(filter, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        filter.FlagType = flagType;
        dbContext.MailingFilterTeams.Update(filter);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SetUserMailingFilter(
        int mailingId,
        long ownerId,
        long userId,
        MailingFilterFlagType flagType,
        CancellationToken cancellationToken)
    {
        var mailingProfile = await GetProfileByMailingIdUserIdOrDefaultAsync(mailingId, ownerId, cancellationToken);
        if (mailingProfile is null)
        {
            mailingProfile = new MailingProfile
            {
                MailingId = mailingId,
                OwnerId = ownerId,
                Users =
                [
                    new MailingFilterUser
                    {
                        UserId = userId,
                        FlagType = flagType
                    }
                ]
            };
            await dbContext.MailingProfiles.AddAsync(mailingProfile, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var filter = await dbContext
            .MailingFilterUsers
            .SingleOrDefaultAsync(x =>
                    x.MailingProfileId == mailingProfile.Id &&
                    x.UserId == userId,
                cancellationToken);
        if (filter is null)
        {
            filter = new MailingFilterUser
            {
                MailingProfileId = mailingProfile.Id,
                UserId = userId,
                FlagType = flagType
            };
            await dbContext.MailingFilterUsers.AddAsync(filter, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        filter.FlagType = flagType;
        dbContext.MailingFilterUsers.Update(filter);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SetBotTypeMailingFilter(int mailingId, long ownerId, BotType botType, MailingFilterFlagType flag, CancellationToken cancellationToken)
    {
        var mailingProfile = await GetProfileByMailingIdUserIdOrDefaultAsync(mailingId, ownerId, cancellationToken);
        if (mailingProfile is null)
        {
            mailingProfile = new MailingProfile
            {
                MailingId = mailingId,
                OwnerId = ownerId,
                BotTypes = new Dictionary<BotType, MailingFilterFlagType>
                {
                    [botType] = flag
                }
            };
            await dbContext.MailingProfiles.AddAsync(mailingProfile, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        mailingProfile.BotTypes[botType] = flag;
        dbContext.MailingProfiles.Update(mailingProfile);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SetBotMailingFilter(
        int mailingId,
        long ownerId,
        int botId,
        MailingFilterFlagType flagType,
        CancellationToken cancellationToken)
    {
        var mailingProfile = await GetProfileByMailingIdUserIdOrDefaultAsync(mailingId, ownerId, cancellationToken);
        if (mailingProfile is null)
        {
            mailingProfile = new MailingProfile
            {
                MailingId = mailingId,
                OwnerId = ownerId,
                Bots =
                [
                    new MailingFilterBot
                    {
                        BotId = botId,
                        FlagType = flagType
                    }
                ]
            };
            await dbContext.MailingProfiles.AddAsync(mailingProfile, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var filter = await dbContext
            .MailingFilterBots
            .SingleOrDefaultAsync(x =>
                    x.MailingProfileId == mailingProfile.Id &&
                    x.BotId == botId,
                cancellationToken);
        if (filter is null)
        {
            filter = new MailingFilterBot
            {
                MailingProfileId = mailingProfile.Id,
                BotId = botId,
                FlagType = flagType
            };
            await dbContext.MailingFilterBots.AddAsync(filter, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        filter.FlagType = flagType;
        dbContext.MailingFilterBots.Update(filter);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<User> GetRecipientsQuery(
        int[] includeGameIds,
        int[] excludeGameIds,
        int[] includeSessionIds,
        int[] excludeSessionIds,
        int[] includeTeamIds,
        int[] excludeTeamIds,
        long[] includeUserIds,
        long[] excludeUserIds)
    {
        var usersQuery = dbContext
            .Users
            .AsQueryable();

        if (includeGameIds.Length != 0)
        {
            usersQuery = usersQuery.Where(x =>
                x.Session != null &&
                includeGameIds.Contains(x.Session.Game.Id));
        }

        if (excludeGameIds.Length != 0)
        {
            usersQuery = usersQuery.Where(x =>
                x.Session == null ||
                !excludeGameIds.Contains(x.Session.Game.Id));
        }

        if (includeSessionIds.Length != 0)
        {
            usersQuery = usersQuery.Where(x =>
                x.Session != null &&
                includeSessionIds.Contains(x.Session.Id));
        }

        if (excludeSessionIds.Length != 0)
        {
            usersQuery = usersQuery.Where(x =>
                x.Session == null ||
                !excludeSessionIds.Contains(x.Session.Id));
        }

        if (includeTeamIds.Length != 0)
        {
            usersQuery = usersQuery.Where(x =>
                x.TeamMembership.Any(e => includeTeamIds.Contains(e.TeamId)));
        }

        if (excludeTeamIds.Length != 0)
        {
            usersQuery = usersQuery.Where(x =>
                x.TeamMembership.All(e => !excludeTeamIds.Contains(e.TeamId)));
        }

        if (includeUserIds.Length != 0)
        {
            usersQuery = usersQuery.Where(x => includeUserIds.Contains(x.Id));
        }

        if (excludeUserIds.Length != 0)
        {
            usersQuery = usersQuery.Where(x => !excludeUserIds.Contains(x.Id));
        }

        return usersQuery;
    }
}