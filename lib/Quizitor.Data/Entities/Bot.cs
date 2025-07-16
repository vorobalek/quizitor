using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Enums;

namespace Quizitor.Data.Entities;

[Table("bot", Schema = "public")]
[PrimaryKey(nameof(Id))]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Bot
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public BotType Type { get; set; }

    [MaxLength(80)]
    public string Token { get; set; } = null!;

    public bool IsActive { get; set; }

    [MaxLength(80)]
    public string Name { get; set; } = null!;

    [MaxLength(80)]
    public string? Username { get; set; }

    public bool DropPendingUpdates { get; set; }

    [InverseProperty(nameof(User.GameServer))]
    public ICollection<User> GameServerUsers { get; set; } = [];

    [InverseProperty(nameof(User.GameAdmin))]
    public ICollection<User> GameAdminUsers { get; set; } = [];

    [InverseProperty(nameof(UserPrompt.Bot))]
    public ICollection<UserPrompt> Prompts { get; set; } = [];

    [InverseProperty(nameof(MailingFilterBot.Bot))]
    public ICollection<MailingFilterBot> MailingFilters { get; set; } = [];
}