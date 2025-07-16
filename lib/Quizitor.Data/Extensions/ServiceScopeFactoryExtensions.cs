using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Quizitor.Data.Extensions;

public static class ServiceScopeFactoryExtensions
{
    public static async Task ExecuteUnitOfWorkWithRetryAsync(
        this IServiceScopeFactory serviceScopeFactory,
        Func<AsyncServiceScope, Task> action,
        CancellationToken cancellationToken,
        int maxRetryCount = 10,
        IsolationLevel isolationLevel = IsolationLevel.Snapshot)
    {
        var retryCount = 0;
        while (retryCount < maxRetryCount && !cancellationToken.IsCancellationRequested)
            try
            {
                await using var asyncScope = serviceScopeFactory.CreateAsyncScope();
                await using var transaction = await asyncScope.ServiceProvider.GetRequiredService<ApplicationDbContext>()
                    .Database
                    .BeginTransactionAsync(isolationLevel, cancellationToken);
                var dbContextProvider = asyncScope.ServiceProvider.GetRequiredService<IDbContextProvider>();

                dbContextProvider.ClearPostCommitTasks();

                await action(asyncScope);
                await transaction.CommitAsync(cancellationToken);

                await Task.WhenAll(dbContextProvider.PostCommitTasks.Select(x => x.Invoke()));
                return;
            }
            catch (Exception exception) when (DbContextProviderExtensions.IsDbUpdateConcurrencyException(exception))
            {
                retryCount++;
            }

        throw new InvalidOperationException($"Exceeded max retry attempts {maxRetryCount}.");
    }
}