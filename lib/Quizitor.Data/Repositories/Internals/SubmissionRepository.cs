using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;

namespace Quizitor.Data.Repositories.Internals;

internal sealed class SubmissionRepository(
    ApplicationDbContext dbContext) : ISubmissionRepository
{
    public async Task AddAsync(Submission submission, CancellationToken cancellationToken)
    {
        await dbContext.Submissions.AddAsync(submission, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(Submission submission, CancellationToken cancellationToken)
    {
        dbContext.Submissions.Update(submission);
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ClearBySessionIdAsync(int sessionId, CancellationToken cancellationToken)
    {
        var submissions = await dbContext
            .Submissions
            .Where(s => s.SessionId == sessionId)
            .ToArrayAsync(cancellationToken);
        dbContext.Submissions.RemoveRange(submissions);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<IGrouping<Team?, Submission>[]> GetForRatingCalculationGroupByTeamNotNullNoTrackingAsync(
        int sessionId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Submissions
            .Where(x => x.SessionId == sessionId && x.TeamId != null)
            .Include(x => x.Question)
            .ThenInclude(x => x.Round)
            .Include(x => x.User)
            .GroupBy(x => x.Team)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);
    }

    public Task<IGrouping<User, Submission>[]> GetForRatingCalculationGroupByUserWithoutTeamNoTrackingAsync(
        int sessionId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Submissions
            .Where(x => x.SessionId == sessionId && x.TeamId == null)
            .Include(x => x.Question)
            .ThenInclude(x => x.Round)
            .Include(x => x.User)
            .GroupBy(x => x.User)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> CountForUserIdWithoutTeamByQuestionIdAndSessionIdAsync(
        long userId,
        int questionId,
        int sessionId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Submissions
            .CountAsync(x =>
                    x.UserId == userId &&
                    x.QuestionId == questionId &&
                    x.SessionId == sessionId &&
                    x.TeamId == null,
                cancellationToken);
    }

    public Task<int> CountForTeamIdByQuestionIdAndSessionIdAsync(
        int teamId,
        int questionId,
        int sessionId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Submissions
            .CountAsync(x =>
                    x.TeamId == teamId &&
                    x.QuestionId == questionId &&
                    x.SessionId == sessionId,
                cancellationToken);
    }

    public Task<int> CountByGameIdAsync(
        int gameId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Submissions
            .CountAsync(
                x => x.Session.GameId == gameId,
                cancellationToken);
    }

    public Task<int> CountByUserIdAsync(long userId, CancellationToken cancellationToken)
    {
        return dbContext
            .Submissions
            .CountAsync(
                x => x.UserId == userId,
                cancellationToken);
    }

    public Task<int> CountBySessionIdAsync(int sessionId, CancellationToken cancellationToken)
    {
        return dbContext
            .Submissions
            .CountAsync(
                x => x.SessionId == sessionId,
                cancellationToken);
    }

    public Task<int> CountByUserIdSessionIdAsync(
        long userId,
        int sessionId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Submissions
            .CountAsync(x =>
                    x.UserId == userId &&
                    x.SessionId == sessionId,
                cancellationToken);
    }

    public Task<int> CountAcceptedByQuestionIdSessionIdAsync(
        int questionId,
        int sessionId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Submissions
            .CountAsync(x =>
                x.QuestionId == questionId &&
                x.SessionId == sessionId &&
                x.Score > 0,
                cancellationToken);
    }
}