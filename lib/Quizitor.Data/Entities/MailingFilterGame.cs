using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("mailing_filter_game", Schema = "public")]
[PrimaryKey(nameof(MailingProfileId), nameof(GameId))]
public class MailingFilterGame : MailingFilterBase
{
    [ForeignKey(nameof(MailingProfile))]
    public int MailingProfileId { get; set; }

    [InverseProperty(nameof(Entities.MailingProfile.Games))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public MailingProfile MailingProfile { get; set; } = null!;

    [ForeignKey(nameof(Game))]
    public int GameId { get; set; }

    [InverseProperty(nameof(Entities.Game.MailingFilters))]
    public Game Game { get; set; } = null!;
}