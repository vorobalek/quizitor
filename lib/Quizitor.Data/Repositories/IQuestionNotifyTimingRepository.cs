using Quizitor.Data.Entities.Events;

namespace Quizitor.Data.Repositories;

public interface IQuestionNotifyTimingRepository
{
    Task AddAsync(
        QuestionTimingNotify notifyEvent,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        QuestionTimingNotify notifyEvent,
        CancellationToken cancellationToken);

    Task RemoveAsync(
        QuestionTimingNotify notifyEvent,
        CancellationToken cancellationToken);

    Task<QuestionTimingNotify?> GetByTimingIdOrDefaultAsync(
        int timingId,
        CancellationToken cancellationToken);

    Task<QuestionTimingNotify?> GetCandidateForProcessingAsync(
        DateTimeOffset dateTimeOffset,
        int seconds,
        CancellationToken cancellationToken);

    Task<int> CountByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken);

    Task<QuestionTimingNotify> GetByTimingIdAsync(
        int timingId,
        CancellationToken cancellationToken);
}