using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;

namespace Quizitor.Data.Repositories.Internals;

internal sealed class QuestionTimingRepository(
    ApplicationDbContext dbContext) : IQuestionTimingRepository
{
    public async Task AddAsync(
        QuestionTiming questionTiming,
        CancellationToken cancellationToken)
    {
        await dbContext.QuestionTimings.AddAsync(questionTiming, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        QuestionTiming questionTiming,
        CancellationToken cancellationToken)
    {
        dbContext.QuestionTimings.Update(questionTiming);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<QuestionTiming?> GetByIdOrDefaultAsync(
        int timingId,
        CancellationToken cancellationToken)
    {
        return await dbContext
            .QuestionTimings
            .SingleOrDefaultAsync(
                x => x.Id == timingId,
                cancellationToken);
    }

    public Task<QuestionTiming?> GetActiveBySessionIdOrDefaultAsync(int sessionId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .QuestionTimings
            .SingleOrDefaultAsync(x =>
                    x.SessionId == sessionId &&
                    x.StopTime == null,
                cancellationToken);
    }

    public Task<int> CountByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .QuestionTimings
            .Where(x => x.Session.GameId == gameId)
            .CountAsync(cancellationToken);
    }
}