using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;

namespace Quizitor.Data.Repositories.Internals;

internal sealed class QuestionRepository(
    ApplicationDbContext dbContext) : IQuestionRepository
{
    public async Task<Question> GetByIdAsync(
        int questionId,
        CancellationToken cancellationToken)
    {
        return await GetByIdOrDefaultAsync(
                   questionId,
                   cancellationToken)
               ?? throw new InvalidOperationException(
                   string
                       .Format(
                           Constants.NoQuestionWithIdFound,
                           questionId));
    }

    public async Task<Question?> GetByIdOrDefaultAsync(
        int questionId,
        CancellationToken cancellationToken)
    {
        return await dbContext
            .Questions
            .SingleOrDefaultAsync(
                x => x.Id == questionId,
                cancellationToken);
    }

    public Task<Question[]> GetByRoundIdAsync(
        int roundId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Questions
            .Where(x => x.RoundId == roundId)
            .OrderBy(x => x.Number)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> CountByRoundIdAsync(
        int roundId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Questions
            .Where(x => x.RoundId == roundId)
            .CountAsync(cancellationToken);
    }

    public Task<int> CountByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Questions
            .Where(x => x.Round.GameId == gameId)
            .CountAsync(cancellationToken);
    }

    public Task<Question?> GetActiveBySessionIdOrDefaultAsync(
        int sessionId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Questions
            .SingleOrDefaultAsync(
                x => x.Timings.Any(e =>
                    e.SessionId == sessionId &&
                    e.StopTime == null),
                cancellationToken);
    }

    public Task<Question[]> GetOrderedByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Questions
            .Where(x => x.Round.GameId == gameId)
            .OrderBy(x => x.Round.Number)
            .ThenBy(x => x.Number)
            .ToArrayAsync(cancellationToken);
    }

    public Task<QuestionOption[]> GetOptionsByQuestionIdAsync(int questionId, CancellationToken cancellationToken)
    {
        return dbContext
            .QuestionOptions
            .Where(x => x.QuestionId == questionId)
            .OrderBy(x => x.Number)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> CountOptionsByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .QuestionOptions
            .Where(x => x.Question.Round.GameId == gameId)
            .CountAsync(cancellationToken);
    }

    public Task<QuestionRule[]> GetRulesByQuestionIdAsync(int questionId, CancellationToken cancellationToken)
    {
        return dbContext
            .QuestionRules
            .Where(r => r.QuestionId == questionId)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> CountRulesByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .QuestionRules
            .CountAsync(
                x => x.Question.Round.GameId == gameId,
                cancellationToken);
    }

    public Task UpdateAsync(Question question, CancellationToken cancellationToken)
    {
        dbContext
            .Questions
            .Update(question);

        return dbContext.SaveChangesAsync(cancellationToken);
    }
}