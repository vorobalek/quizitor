using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("mailing_filter_bot", Schema = "public")]
[PrimaryKey(nameof(MailingProfileId), nameof(BotId))]
public class MailingFilterBot : MailingFilterBase
{
    [ForeignKey(nameof(MailingProfile))]
    public int MailingProfileId { get; set; }

    [InverseProperty(nameof(Entities.MailingProfile.Bots))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public MailingProfile MailingProfile { get; set; } = null!;

    [ForeignKey(nameof(User))]
    public int BotId { get; set; }

    [InverseProperty(nameof(Entities.Bot.MailingFilters))]
    public Bot Bot { get; set; } = null!;
}