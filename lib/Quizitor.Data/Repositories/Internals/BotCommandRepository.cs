using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Repositories.Internals;

internal sealed class BotCommandRepository(
    ApplicationDbContext dbContext) : IBotCommandRepository
{
    public async Task AddAsync(
        BotCommand botCommand,
        CancellationToken cancellationToken)
    {
        await dbContext.BotCommands.AddAsync(botCommand, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        BotCommand botCommand,
        CancellationToken cancellationToken)
    {
        dbContext.BotCommands.Update(botCommand);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<BotCommand[]> GetByTypeAsync(
        BotType botType,
        CancellationToken cancellationToken)
    {
        return dbContext
            .BotCommands
            .Where(x => x.BotType == botType)
            .ToArrayAsync(cancellationToken);
    }

    public Task<BotCommand?> GetByTypeAndCommandAsync(
        BotType botType,
        string command,
        CancellationToken cancellationToken)
    {
        return dbContext
            .BotCommands
            .SingleOrDefaultAsync(
                x =>
                    x.BotType == botType &&
                    x.Command == command,
                cancellationToken);
    }
}