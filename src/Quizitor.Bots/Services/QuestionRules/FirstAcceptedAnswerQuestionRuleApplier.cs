using Quizitor.Data;
using Quizitor.Data.Entities;

namespace Quizitor.Bots.Services.QuestionRules;

internal sealed class FirstAcceptedAnswerQuestionRuleApplier(
    IDbContextProvider dbContextProvider) :
    IQuestionRuleApplier<FirstAcceptedAnswerQuestionRule>
{
    public async Task<int?> ApplyAsync(
        FirstAcceptedAnswerQuestionRule questionRule,
        Submission submission,
        CancellationToken cancellationToken)
    {
        var correctSubmissionsCount = await dbContextProvider
            .Submissions
            .CountAcceptedByQuestionIdSessionIdAsync(
                submission.QuestionId,
                submission.SessionId,
                cancellationToken);
        
        return correctSubmissionsCount <= questionRule.Limit 
            ? questionRule.Cost
            : null;
    }
}