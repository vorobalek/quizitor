using Quizitor.Data.Entities;

namespace Quizitor.Data.Repositories;

public interface IQuestionTimingRepository
{
    Task AddAsync(
        QuestionTiming questionTiming,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        QuestionTiming questionTiming,
        CancellationToken cancellationToken);

    Task<QuestionTiming?> GetByIdOrDefaultAsync(
        int timingId,
        CancellationToken cancellationToken);

    Task<QuestionTiming?> GetActiveBySessionIdOrDefaultAsync(
        int sessionId,
        CancellationToken cancellationToken);

    Task<int> CountByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken);
}