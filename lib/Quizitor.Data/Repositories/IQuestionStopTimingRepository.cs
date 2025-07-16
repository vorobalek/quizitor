using Quizitor.Data.Entities.Events;

namespace Quizitor.Data.Repositories;

public interface IQuestionStopTimingRepository
{
    Task AddAsync(
        QuestionTimingStop stopEvent,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        QuestionTimingStop stopEvent,
        CancellationToken cancellationToken);

    Task RemoveAsync(
        QuestionTimingStop stopEvent,
        CancellationToken cancellationToken);

    Task<QuestionTimingStop?> GetByTimingIdOrDefaultAsync(
        int timingId,
        CancellationToken cancellationToken);

    Task<QuestionTimingStop?> GetCandidateForProcessingAsync(
        DateTimeOffset serverDateTime,
        int seconds,
        CancellationToken cancellationToken);

    Task<int> CountByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken);

    Task<QuestionTimingStop> GetByTimingIdAsync(
        int timingId,
        CancellationToken cancellationToken);
}