using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("submission", Schema = "public")]
[PrimaryKey(nameof(Id))]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Submission
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(4096)]
    public string Text { get; set; } = null!;

    public int Time { get; set; }

    [ForeignKey(nameof(Question))]
    public int QuestionId { get; set; }

    [InverseProperty(nameof(Entities.Question.Submissions))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Question Question { get; set; } = null!;

    [ForeignKey(nameof(User))]
    public long UserId { get; set; }

    [InverseProperty(nameof(Entities.User.Submissions))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(Team))]
    public int? TeamId { get; set; }

    [InverseProperty(nameof(Entities.Team.Submissions))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Team? Team { get; set; }

    [ForeignKey(nameof(Session))]
    public int SessionId { get; set; }

    [InverseProperty(nameof(Entities.Session.Submissions))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Session Session { get; set; } = null!;

    public int Score { get; set; }
}