using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("mailing", Schema = "public")]
[PrimaryKey(nameof(Id))]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Mailing
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(256)]
    public string Name { get; set; } = null!;

    [MaxLength(4096)]
    public string Text { get; set; } = null!;

    [InverseProperty(nameof(MailingProfile.Mailing))]
    public ICollection<MailingProfile> Profiles { get; set; } = [];
}