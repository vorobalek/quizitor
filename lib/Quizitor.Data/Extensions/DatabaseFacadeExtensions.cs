using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Extensions;

public static class DatabaseFacadeExtensions
{
    public static Task<DateTimeOffset> GetServerDateTimeOffsetAsync(
        this DbContext dbContext,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Database
            .SqlQuery<DateTimeOffset>($"select now() as \"Value\"")
            .SingleOrDefaultAsync(cancellationToken);
    }
}