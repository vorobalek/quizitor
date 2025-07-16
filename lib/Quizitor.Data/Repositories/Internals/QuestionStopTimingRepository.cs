using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities.Events;

namespace Quizitor.Data.Repositories.Internals;

internal sealed class QuestionStopTimingRepository(
    ApplicationDbContext dbContext) : IQuestionStopTimingRepository
{
    public async Task AddAsync(
        QuestionTimingStop stopEvent,
        CancellationToken cancellationToken)
    {
        await dbContext.QuestionStopTimings.AddAsync(stopEvent, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        QuestionTimingStop stopEvent,
        CancellationToken cancellationToken)
    {
        dbContext.QuestionStopTimings.Update(stopEvent);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(
        QuestionTimingStop stopEvent,
        CancellationToken cancellationToken)
    {
        dbContext.QuestionStopTimings.Remove(stopEvent);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<QuestionTimingStop?> GetByTimingIdOrDefaultAsync(
        int timingId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .QuestionStopTimings
            .SingleOrDefaultAsync(
                x => x.TimingId == timingId,
                cancellationToken);
    }

    public Task<QuestionTimingStop?> GetCandidateForProcessingAsync(
        DateTimeOffset dateTimeOffset,
        int seconds,
        CancellationToken cancellationToken)
    {
        return dbContext
            .QuestionStopTimings
            .Where(x =>
                (x.LastRunAt == null ||
                 x.LastRunAt.Value.AddSeconds(seconds) < dateTimeOffset) &&
                x.RunAt < dateTimeOffset)
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<int> CountByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .QuestionStopTimings
            .Where(x => x.Timing.Session.GameId == gameId)
            .CountAsync(cancellationToken);
    }

    public Task<QuestionTimingStop> GetByTimingIdAsync(
        int timingId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .QuestionStopTimings
            .SingleAsync(
                x => x.TimingId == timingId,
                cancellationToken);
    }
}