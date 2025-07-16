using Quizitor.Data.Enums;

namespace Quizitor.Data.Entities;

public abstract class MailingFilterBase
{
    public MailingFilterFlagType FlagType { get; set; }
}