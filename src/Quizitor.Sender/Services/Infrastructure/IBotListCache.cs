using Quizitor.Data.Entities;

namespace Quizitor.Sender.Services.Infrastructure;

internal interface IBotListCache
{
    Task<Bot[]> GetBotsAsync(CancellationToken cancellationToken);
}
