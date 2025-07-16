using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("mailing_filter_user", Schema = "public")]
[PrimaryKey(nameof(MailingProfileId), nameof(UserId))]
public class MailingFilterUser : MailingFilterBase
{
    [ForeignKey(nameof(MailingProfile))]
    public int MailingProfileId { get; set; }

    [InverseProperty(nameof(Entities.MailingProfile.Users))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public MailingProfile MailingProfile { get; set; } = null!;

    [ForeignKey(nameof(User))]
    public long UserId { get; set; }

    [InverseProperty(nameof(Entities.User.MailingFilters))]
    public User User { get; set; } = null!;
}