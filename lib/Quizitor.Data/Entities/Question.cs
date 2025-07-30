using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Entities;

[Table("question", Schema = "public")]
[PrimaryKey(nameof(Id))]
[Index(nameof(RoundId), nameof(Number), IsUnique = true)]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Question
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(Round))]
    public int RoundId { get; set; }

    [InverseProperty(nameof(Entities.Round.Questions))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Round Round { get; set; } = null!;

    public int Number { get; set; }

    [MaxLength(256)]
    public string Title { get; set; } = null!;

    [MaxLength(1024)]
    public string Text { get; set; } = null!;

    [MaxLength(1024)]
    public string? Comment { get; set; }

    public int Time { get; set; }

    public int? NotificationTime { get; set; }

    public bool AutoClose { get; set; }

    public int Attempts { get; set; }

    public SubmissionNotificationType SubmissionNotificationType { get; set; }

    [InverseProperty(nameof(QuestionOption.Question))]
    public ICollection<QuestionOption> Options { get; set; } = [];

    [InverseProperty(nameof(QuestionRule.Question))]
    public ICollection<QuestionRule> Rules { get; set; } = [];

    [InverseProperty(nameof(Submission.Question))]
    public ICollection<Submission> Submissions { get; set; } = [];

    [InverseProperty(nameof(QuestionTiming.Question))]
    public ICollection<QuestionTiming> Timings { get; set; } = [];
}