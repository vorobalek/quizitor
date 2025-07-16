using Quizitor.Data.Entities;

namespace Quizitor.Data.Repositories;

public interface ISubmissionRepository
{
    Task AddAsync(Submission submission, CancellationToken cancellationToken);
    Task UpdateAsync(Submission submission, CancellationToken cancellationToken);
    Task ClearBySessionIdAsync(int sessionId, CancellationToken cancellationToken);

    Task<IGrouping<Team?, Submission>[]> GetForRatingCalculationGroupByTeamNotNullNoTrackingAsync(
        int sessionId,
        CancellationToken cancellationToken);

    Task<IGrouping<User, Submission>[]> GetForRatingCalculationGroupByUserWithoutTeamNoTrackingAsync(
        int sessionId,
        CancellationToken cancellationToken);

    Task<int> CountForUserIdWithoutTeamByQuestionIdAndSessionIdAsync(
        long userId,
        int questionId,
        int sessionId,
        CancellationToken cancellationToken);

    Task<int> CountForTeamIdByQuestionIdAndSessionIdAsync(
        int teamId,
        int questionId,
        int sessionId,
        CancellationToken cancellationToken);

    Task<int> CountByGameIdAsync(int gameId, CancellationToken cancellationToken);

    Task<int> CountByUserIdAsync(long userId, CancellationToken cancellationToken);
    Task<int> CountBySessionIdAsync(int sessionId, CancellationToken cancellationToken);
    Task<int> CountByUserIdSessionIdAsync(long userId, int sessionId, CancellationToken cancellationToken);
    Task<int> CountAcceptedByQuestionIdSessionIdAsync(int questionId, int sessionId, CancellationToken cancellationToken);
}