using Quizitor.Data.Entities;

namespace Quizitor.Bots.Services.QuestionRules;

internal interface IQuestionRuleApplier
{
    Type TargetType { get; }

    Task<int?> ApplyAsync(
        QuestionRule questionRule,
        Submission submission,
        CancellationToken cancellationToken);
}

internal interface IQuestionRuleApplier<in TQuestionRule> : IQuestionRuleApplier
    where TQuestionRule : QuestionRule
{
    Type IQuestionRuleApplier.TargetType => typeof(TQuestionRule);

    Task<int?> IQuestionRuleApplier.ApplyAsync(
        QuestionRule questionRule,
        Submission submission,
        CancellationToken cancellationToken)
    {
        return ApplyAsync((TQuestionRule)questionRule, submission, cancellationToken);
    }

    Task<int?> ApplyAsync(
        TQuestionRule questionRule,
        Submission submission,
        CancellationToken cancellationToken);
}