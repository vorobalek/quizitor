using Quizitor.Data;
using Quizitor.Data.Entities;

namespace Quizitor.Sender.Services.Infrastructure;

internal sealed class BotListCache : IBotListCache
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private Bot[]? _bots;
    private DateTimeOffset _lastRefresh = DateTimeOffset.MinValue;

    public BotListCache(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<Bot[]> GetBotsAsync(CancellationToken cancellationToken)
    {
        if (IsCacheValid())
        {
            return _bots!;
        }

        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            if (IsCacheValid())
            {
                return _bots!;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IDbContextProvider>();
            var bots = await dbContext.Bots.GetAllAsync(cancellationToken);
            _bots = bots;
            _lastRefresh = DateTimeOffset.UtcNow;
            return bots;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private bool IsCacheValid()
    {
        return _bots is not null && DateTimeOffset.UtcNow - _lastRefresh < CacheTtl;
    }
}
