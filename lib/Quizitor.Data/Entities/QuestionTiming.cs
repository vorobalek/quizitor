using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("question_timing", Schema = "public")]
[PrimaryKey(nameof(Id))]
public class QuestionTiming
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(Session))]
    public int SessionId { get; set; }

    [InverseProperty(nameof(Entities.Session.Timings))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Session Session { get; set; } = null!;

    [ForeignKey(nameof(Question))]
    public int QuestionId { get; set; }

    [InverseProperty(nameof(Entities.Question.Timings))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Question Question { get; set; } = null!;

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset? StopTime { get; set; }
}