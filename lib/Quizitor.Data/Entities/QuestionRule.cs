using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("question_rule", Schema = "public")]
[PrimaryKey(nameof(Id))]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public abstract class QuestionRule
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    [InverseProperty(nameof(Entities.Question.Rules))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Question Question { get; set; } = null!;

    public int Cost { get; set; }

    public abstract object?[] GetBackOfficeLocalizationArgs();
    public abstract object?[] GetGameAdminLocalizationArgs();
}