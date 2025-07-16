using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;

namespace Quizitor.Data.Repositories.Internals;

internal sealed class BotInteractionRepository(
    ApplicationDbContext dbContext) : IBotInteractionRepository
{
    public async Task UpsertAsync(
        string botUsername,
        long userId,
        DateTimeOffset lastInteraction,
        CancellationToken cancellationToken)
    {
        var interaction = await dbContext
            .BotInteractions
            .Where(x =>
                x.BotUsername == botUsername &&
                x.UserId == userId)
            .SingleOrDefaultAsync(cancellationToken);

        if (interaction is null)
        {
            await dbContext
                .BotInteractions
                .AddAsync(
                    new BotInteraction
                    {
                        BotUsername = botUsername,
                        UserId = userId,
                        LastInteraction = lastInteraction
                    },
                    cancellationToken);
        }
        else
        {
            interaction.LastInteraction = lastInteraction;
            dbContext.BotInteractions.Update(interaction);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<BotInteraction[]> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .BotInteractions
            .Where(x => x.UserId == userId)
            .ToArrayAsync(cancellationToken);
    }
}