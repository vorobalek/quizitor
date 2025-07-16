using Quizitor.Data.Entities;

namespace Quizitor.Data.Repositories;

public interface IBotInteractionRepository
{
    Task UpsertAsync(
        string botUsername,
        long userId,
        DateTimeOffset lastInteraction,
        CancellationToken cancellationToken);

    Task<BotInteraction[]> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken);
}