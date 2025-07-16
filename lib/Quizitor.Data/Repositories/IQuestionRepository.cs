using Quizitor.Data.Entities;

namespace Quizitor.Data.Repositories;

public interface IQuestionRepository
{
    Task<Question> GetByIdAsync(
        int questionId,
        CancellationToken cancellationToken);

    Task<Question?> GetByIdOrDefaultAsync(
        int questionId,
        CancellationToken cancellationToken);

    Task<Question[]> GetByRoundIdAsync(
        int roundId,
        CancellationToken cancellationToken);

    Task<int> CountByRoundIdAsync(
        int roundId,
        CancellationToken cancellationToken);

    Task<int> CountByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken);

    Task<Question?> GetActiveBySessionIdOrDefaultAsync(
        int sessionId,
        CancellationToken cancellationToken);

    Task<Question[]> GetOrderedByGameIdAsync(int gameId, CancellationToken cancellationToken);
    Task<QuestionOption[]> GetOptionsByQuestionIdAsync(int questionId, CancellationToken cancellationToken);
    Task<int> CountOptionsByGameIdAsync(int gameId, CancellationToken cancellationToken);
    Task<QuestionRule[]> GetRulesByQuestionIdAsync(int questionId, CancellationToken cancellationToken);
    Task<int> CountRulesByGameIdAsync(int gameId, CancellationToken cancellationToken);
}