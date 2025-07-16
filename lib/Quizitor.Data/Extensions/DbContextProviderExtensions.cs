using System.Data;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Extensions;

public static class DbContextProviderExtensions
{
    public static bool IsDbUpdateConcurrencyException(Exception exception)
    {
        return exception is DbUpdateConcurrencyException or DbUpdateException ||
               exception.InnerException is not null && IsDbUpdateConcurrencyException(exception.InnerException);
    }

    public static async Task ExecuteInTransactionAsync(
        this IDbContextProvider dbContextProvider,
        Func<Task> action,
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