using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("game", Schema = "public")]
[PrimaryKey(nameof(Id))]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Game
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(256)]
    public string Title { get; set; } = null!;

    [InverseProperty(nameof(Round.Game))]
    public ICollection<Round> Rounds { get; set; } = [];

    [InverseProperty(nameof(Session.Game))]
    public ICollection<Session> Sessions { get; set; } = [];

    [InverseProperty(nameof(MailingFilterGame.Game))]
    public ICollection<MailingFilterGame> MailingFilters { get; set; } = [];
}