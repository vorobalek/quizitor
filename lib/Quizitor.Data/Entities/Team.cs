using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("team", Schema = "public")]
[PrimaryKey(nameof(Id))]
[Index(nameof(Name), IsUnique = true)]
public class Team
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(80)]
    public string Name { get; set; } = null!;

    [ForeignKey(nameof(Owner))]
    public long OwnerId { get; set; }

    [InverseProperty(nameof(User.OwnedTeams))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User Owner { get; set; } = null!;

    [InverseProperty(nameof(Submission.Team))]
    public ICollection<Submission> Submissions { get; set; } = [];

    [InverseProperty(nameof(TeamMember.Team))]
    public ICollection<TeamMember> Members { get; set; } = [];

    [InverseProperty(nameof(TeamLeader.Team))]
    public ICollection<TeamLeader> Leaders { get; set; } = [];

    [InverseProperty(nameof(MailingFilterTeam.Team))]
    public ICollection<MailingFilterTeam> MailingFilters { get; set; } = [];
}