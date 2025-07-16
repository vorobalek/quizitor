namespace Quizitor.Data.Entities;

public sealed class AnyAnswerQuestionRule : QuestionRule
{
    public override object?[] GetBackOfficeLocalizationArgs()
    {
        return [];
    }

    public override object?[] GetGameAdminLocalizationArgs()
    {
        return [];
    }
}