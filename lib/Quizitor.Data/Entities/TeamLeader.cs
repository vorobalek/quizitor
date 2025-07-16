using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("team_leader", Schema = "public")]
[PrimaryKey(nameof(TeamId), nameof(SessionId))]
[Index(nameof(SessionId), nameof(UserId), IsUnique = true)]
public class TeamLeader
{
    [ForeignKey(nameof(Team))]
    public int TeamId { get; set; }

    [InverseProperty(nameof(Entities.Team.Leaders))]
    public Team Team { get; set; } = null!;

    [ForeignKey(nameof(Session))]
    public int SessionId { get; set; }

    [InverseProperty(nameof(Entities.Session.TeamLeaders))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Session Session { get; set; } = null!;

    [ForeignKey(nameof(User))]
    public long UserId { get; set; }

    [InverseProperty(nameof(Entities.User.TeamLeadership))]
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public User User { get; set; } = null!;
}