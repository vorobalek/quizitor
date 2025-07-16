namespace Quizitor.Data.Entities;

public sealed class FirstAcceptedAnswerQuestionRule : QuestionRule
{
    public int Limit { get; set; }

    public override object?[] GetBackOfficeLocalizationArgs()
    {
        return [Limit];
    }

    public override object?[] GetGameAdminLocalizationArgs()
    {
        return [Limit];
    }
}