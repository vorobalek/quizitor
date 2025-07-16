using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("team_member", Schema = "public")]
[PrimaryKey(nameof(UserId), nameof(SessionId))]
public class TeamMember
{
    [ForeignKey(nameof(User))]
    public long UserId { get; set; }

    [InverseProperty(nameof(Entities.User.TeamMembership))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(Session))]
    public int SessionId { get; set; }

    [InverseProperty(nameof(Entities.Session.TeamMembers))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Session Session { get; set; } = null!;

    [ForeignKey(nameof(Team))]
    public int TeamId { get; set; }

    [InverseProperty(nameof(Entities.Team.Members))]
    public Team Team { get; set; } = null!;
}