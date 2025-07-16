using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("mailing_filter_team", Schema = "public")]
[PrimaryKey(nameof(MailingProfileId), nameof(TeamId))]
public class MailingFilterTeam : MailingFilterBase
{
    [ForeignKey(nameof(MailingProfile))]
    public int MailingProfileId { get; set; }

    [InverseProperty(nameof(Entities.MailingProfile.Teams))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public MailingProfile MailingProfile { get; set; } = null!;

    [ForeignKey(nameof(Team))]
    public int TeamId { get; set; }

    [InverseProperty(nameof(Entities.Team.MailingFilters))]
    public Team Team { get; set; } = null!;
}