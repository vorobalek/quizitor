using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("bot_interaction", Schema = "public")]
[PrimaryKey(nameof(BotUsername), nameof(UserId))]
public class BotInteraction
{
    [MaxLength(80)]
    public string BotUsername { get; set; } = null!;

    [ForeignKey(nameof(User))]
    public long UserId { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; } = null!;

    public DateTimeOffset LastInteraction { get; set; }
}