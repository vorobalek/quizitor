using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("mailing_filter_session", Schema = "public")]
[PrimaryKey(nameof(MailingProfileId), nameof(SessionId))]
public class MailingFilterSession : MailingFilterBase
{
    [ForeignKey(nameof(MailingProfile))]
    public int MailingProfileId { get; set; }

    [InverseProperty(nameof(Entities.MailingProfile.Sessions))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public MailingProfile MailingProfile { get; set; } = null!;

    [ForeignKey(nameof(Session))]
    public int SessionId { get; set; }

    [InverseProperty(nameof(Entities.Session.MailingFilters))]
    public Session Session { get; set; } = null!;
}