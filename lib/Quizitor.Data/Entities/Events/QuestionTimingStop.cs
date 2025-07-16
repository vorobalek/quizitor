using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities.Events;

[Table("question_timing_stop", Schema = "events")]
[PrimaryKey(nameof(Id))]
[Index(nameof(TimingId), IsUnique = true)]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class QuestionTimingStop
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(Timing))]
    public int TimingId { get; set; }

    public QuestionTiming Timing { get; set; } = null!;

    public DateTimeOffset RunAt { get; set; }

    public DateTimeOffset? LastRunAt { get; set; }

    public int RoundPageNumber { get; set; }

    public int QuestionPageNumber { get; set; }
}