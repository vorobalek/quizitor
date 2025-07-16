using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Entities;

[Table("bot_command", Schema = "public")]
[PrimaryKey(nameof(Id))]
[Index(nameof(BotType), nameof(Command), IsUnique = true)]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class BotCommand
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public BotType BotType { get; set; }

    [MaxLength(32)]
    public string Command { get; set; } = null!;

    [MaxLength(256)]
    public string Description { get; set; } = null!;
}