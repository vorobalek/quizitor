using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Repositories;

public interface IBotCommandRepository
{
    Task AddAsync(
        BotCommand botCommand,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        BotCommand botCommand,
        CancellationToken cancellationToken);

    Task<BotCommand[]> GetByTypeAsync(
        BotType botType,
        CancellationToken cancellationToken);

    Task<BotCommand?> GetByTypeAndCommandAsync(
        BotType botType,
        string command,
        CancellationToken cancellationToken);
}