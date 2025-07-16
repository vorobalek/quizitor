using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities.Events;

namespace Quizitor.Data.Repositories.Internals;

internal sealed class QuestionNotifyTimingRepository(
    ApplicationDbContext dbContext) : IQuestionNotifyTimingRepository
{
    public async Task AddAsync(
        QuestionTimingNotify notifyEvent,
        CancellationToken cancellationToken)
    {
        await dbContext.QuestionNotifyTimings.AddAsync(notifyEvent, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        QuestionTimingNotify notifyEvent,
        CancellationToken cancellationToken)
    {
        dbContext.QuestionNotifyTimings.Update(notifyEvent);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(
        QuestionTimingNotify notifyEvent,
        CancellationToken cancellationToken)
    {
        dbContext
            .QuestionNotifyTimings
            .Remove(notifyEvent);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<QuestionTimingNotify?> GetByTimingIdOrDefaultAsync(
        int timingId,
        CancellationToken cancellationToken)
    {
        return await dbContext
            .QuestionNotifyTimings
            .SingleOrDefaultAsync(
                x => x.TimingId == timingId,
                cancellationToken);
    }

    public Task<QuestionTimingNotify?> GetCandidateForProcessingAsync(
        DateTimeOffset dateTimeOffset,
        int seconds,
        CancellationToken cancellationToken)
    {
        return dbContext
            .QuestionNotifyTimings
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
            .QuestionNotifyTimings
            .Where(x => x.Timing.Session.GameId == gameId)
            .CountAsync(cancellationToken);
    }

    public Task<QuestionTimingNotify> GetByTimingIdAsync(
        int timingId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .QuestionNotifyTimings
            .SingleAsync(x =>
                    x.TimingId == timingId,
                cancellationToken);
    }
}