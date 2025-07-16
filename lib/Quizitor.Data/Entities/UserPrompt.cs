using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Entities;

[Table("user_prompt", Schema = "public")]
[PrimaryKey(nameof(Id))]
[Index(nameof(UserId), nameof(BotId), IsUnique = true)]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class UserPrompt
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(User))]
    public long UserId { get; set; }

    [InverseProperty(nameof(User.Prompts))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(Bot))]
    public int? BotId { get; set; }

    [InverseProperty(nameof(Entities.Bot.Prompts))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Bot? Bot { get; set; }

    public UserPromptType Type { get; set; }

    [MaxLength(1024)]
    public string Subject { get; set; } = null!;
}