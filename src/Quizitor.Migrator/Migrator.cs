using Microsoft.EntityFrameworkCore;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Extensions;
using Quizitor.Migrator.Seeds;

namespace Quizitor.Migrator;

internal sealed class Migrator(
    IDbContextProvider dbContextProvider,
    ApplicationDbContext dbContext,
    IEnumerable<ISeed> seeds,
    IGlobalCancellationTokenSource globalCancellationTokenSource) : IMigrator
{
    public async Task MigrateAsync()
    {
        await MigrateSchemaAsync(globalCancellationTokenSource.Token);
        await SeedDatabaseAsync(globalCancellationTokenSource.Token);
    }

    private async Task MigrateSchemaAsync(CancellationToken cancellationToken)
    {
        if (dbContext.Database.IsRelational())
            await dbContext.Database.MigrateAsync(cancellationToken);
    }

    private async Task SeedDatabaseAsync(CancellationToken cancellationToken)
    {
        await dbContextProvider.ExecuteInTransactionAsync(
            async () =>
            {
                foreach (var seed in seeds) await seed.ApplyAsync(cancellationToken);
            },
            cancellationToken);
    }
}