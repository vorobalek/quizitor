using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Extensions;

public static class DatabaseFacadeExtensions
{
    extension(DbContext dbContext)
    {
        public Task<DateTimeOffset> GetServerDateTimeOffsetAsync(CancellationToken cancellationToken)
        {
            return dbContext
                .Database
                .SqlQuery<DateTimeOffset>($"select now() as \"Value\"")
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}