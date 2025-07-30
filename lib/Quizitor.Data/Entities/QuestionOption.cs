using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("question_option", Schema = "public")]
[PrimaryKey(nameof(Id))]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class QuestionOption
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(Question))]
    public int QuestionId { get; set; }

    [InverseProperty(nameof(Entities.Question.Options))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Question Question { get; set; } = null!;

    public int Number { get; set; }

    [MaxLength(256)]
    public string Text { get; set; } = null!;

    public int Cost { get; set; }
}