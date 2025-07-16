using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Entities;

[PrimaryKey(nameof(Id))]
[Table("mailing_profile", Schema = "public")]
[Index(nameof(MailingId), nameof(OwnerId), IsUnique = true)]
public class MailingProfile
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(Mailing))]
    public int MailingId { get; set; }

    [InverseProperty(nameof(Entities.Mailing.Profiles))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Mailing Mailing { get; set; } = null!;

    [ForeignKey(nameof(Owner))]
    public long OwnerId { get; set; }

    [InverseProperty(nameof(User.OwnedMailingFilters))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User Owner { get; set; } = null!;

    public MailingProfileContactType ContactType { get; set; }

    [Column(TypeName = "jsonb")]
    public Dictionary<BotType, MailingFilterFlagType> BotTypes { get; set; } = [];

    [InverseProperty(nameof(MailingFilterGame.MailingProfile))]
    public ICollection<MailingFilterGame> Games { get; set; } = [];

    [InverseProperty(nameof(MailingFilterSession.MailingProfile))]
    public ICollection<MailingFilterSession> Sessions { get; set; } = [];

    [InverseProperty(nameof(MailingFilterTeam.MailingProfile))]
    public ICollection<MailingFilterTeam> Teams { get; set; } = [];

    [InverseProperty(nameof(MailingFilterUser.MailingProfile))]
    public ICollection<MailingFilterUser> Users { get; set; } = [];

    [InverseProperty(nameof(MailingFilterBot.MailingProfile))]
    public ICollection<MailingFilterBot> Bots { get; set; } = [];
}