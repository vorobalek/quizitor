using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Quizitor.Data.Extensions;

public static class ServiceScopeFactoryExtensions
{
    extension(IServiceScopeFactory serviceScopeFactory)
    {
        public async Task ExecuteUnitOfWorkWithRetryAsync(
            Func<IServiceProvider, Task> action,
            CancellationToken cancellationToken,
            int maxRetryCount = 10,
            IsolationLevel isolationLevel = IsolationLevel.Snapshot)
        {
            var retryCount = 0;
            while (retryCount < maxRetryCount && !cancellationToken.IsCancellationRequested)
                try
                {
                    await using var asyncScope = serviceScopeFactory.CreateAsyncScope();
                    var services = asyncScope.ServiceProvider;
                    var dbContextProvider = services.GetRequiredService<IDbContextProvider>();
                    dbContextProvider.ClearPostCommitTasks();

                    await using var transaction = await dbContextProvider.BeginTransactionAsync(isolationLevel, cancellationToken);
                    await action(services);
                    await transaction.CommitAsync(cancellationToken);

                    await Task.WhenAll(dbContextProvider.PostCommitTasks.Select(x => x.Invoke()));
                    return;
                }
                catch (Exception exception) when (exception.IsDbUpdateConcurrencyException)
                {
                    retryCount++;
                }

            throw new InvalidOperationException($"Exceeded max retry attempts {maxRetryCount}.");
        }
    }
}