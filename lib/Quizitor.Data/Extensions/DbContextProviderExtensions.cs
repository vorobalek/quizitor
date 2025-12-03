using System.Data;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Extensions;

public static class DbContextProviderExtensions
{
    extension(Exception exception)
    {
        public bool IsDbUpdateConcurrencyException =>
            exception is DbUpdateConcurrencyException or DbUpdateException ||
            exception.InnerException is not null && exception.InnerException.IsDbUpdateConcurrencyException;
    }

    extension(IDbContextProvider dbContextProvider)
    {
        public async Task ExecuteInTransactionAsync(Func<Task> action,
            CancellationToken cancellationToken,
            IsolationLevel isolationLevel = IsolationLevel.Snapshot)
        {
            await using var transaction = await dbContextProvider
                .BeginTransactionAsync(
                    isolationLevel,
                    cancellationToken);
            await action();
            await transaction.CommitAsync(cancellationToken);
            await Task.WhenAll(dbContextProvider.PostCommitTasks.Select(x => x.Invoke()));
        }
    }
}