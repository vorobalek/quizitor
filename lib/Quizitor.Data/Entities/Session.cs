using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("session", Schema = "public")]
[PrimaryKey(nameof(Id))]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Session
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(Game))]
    public int GameId { get; set; }

    [InverseProperty(nameof(Game.Sessions))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Game Game { get; set; } = null!;

    [MaxLength(256)]
    public string Name { get; set; } = null!;

    public bool SyncRating { get; set; }

    [InverseProperty(nameof(Submission.Session))]
    public ICollection<Submission> Submissions { get; set; } = [];

    [InverseProperty(nameof(User.Session))]
    public ICollection<User> Users { get; set; } = [];

    [InverseProperty(nameof(QuestionTiming.Session))]
    public ICollection<QuestionTiming> Timings { get; set; } = [];

    [InverseProperty(nameof(TeamMember.Session))]
    public ICollection<TeamMember> TeamMembers { get; set; } = [];

    [InverseProperty(nameof(TeamLeader.Session))]
    public ICollection<TeamLeader> TeamLeaders { get; set; } = [];

    [InverseProperty(nameof(MailingFilterSession.Session))]
    public ICollection<MailingFilterSession> MailingFilters { get; set; } = [];
}