using Quizitor.Data.Entities;

namespace Quizitor.Bots.Services.QuestionRules;

internal sealed class AnyAnswerQuestionRuleApplier : IQuestionRuleApplier<AnyAnswerQuestionRule>
{
    public Task<int?> ApplyAsync(
        AnyAnswerQuestionRule questionRule,
        Submission submission,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(questionRule.Cost);
    }
}