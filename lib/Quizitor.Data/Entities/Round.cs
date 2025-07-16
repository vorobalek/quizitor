using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("round", Schema = "public")]
[PrimaryKey(nameof(Id))]
[Index(nameof(GameId), nameof(Number), IsUnique = true)]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Round
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(Game))]
    public int GameId { get; set; }

    [InverseProperty(nameof(Entities.Game.Rounds))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Game Game { get; set; } = null!;

    public int Number { get; set; }

    [MaxLength(256)]
    public string Title { get; set; } = null!;

    [MaxLength(1024)]
    public string? Description { get; set; }

    [InverseProperty(nameof(Question.Round))]
    public ICollection<Question> Questions { get; set; } = [];
}