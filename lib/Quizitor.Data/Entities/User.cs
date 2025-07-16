using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("user", Schema = "public")]
[PrimaryKey(nameof(Id))]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; init; }

    [MaxLength(80)]
    public string FirstName { get; set; } = null!;

    [MaxLength(80)]
    public string? LastName { get; set; }

    [MaxLength(80)]
    public string? Username { get; set; }

    [InverseProperty(nameof(UserPermission.User))]
    public ICollection<UserPermission> Permissions { get; set; } = [];

    [InverseProperty(nameof(Role.Users))]
    public ICollection<Role> Roles { get; set; } = [];

    [ForeignKey(nameof(GameServer))]
    public int? GameServerId { get; set; }

    [InverseProperty(nameof(Bot.GameServerUsers))]
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public Bot? GameServer { get; set; }

    [ForeignKey(nameof(GameAdmin))]
    public int? GameAdminId { get; set; }

    [InverseProperty(nameof(Bot.GameAdminUsers))]
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public Bot? GameAdmin { get; set; }

    [ForeignKey(nameof(Session))]
    public int? SessionId { get; set; }

    [InverseProperty(nameof(Entities.Session.Users))]
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public Session? Session { get; set; }

    [InverseProperty(nameof(Submission.User))]
    public ICollection<Submission> Submissions { get; set; } = [];

    [InverseProperty(nameof(UserPrompt.User))]
    public ICollection<UserPrompt> Prompts { get; set; } = [];

    [InverseProperty(nameof(Team.Owner))]
    public ICollection<Team> OwnedTeams { get; set; } = [];

    [InverseProperty(nameof(TeamMember.User))]
    public ICollection<TeamMember> TeamMembership { get; set; } = [];

    [InverseProperty(nameof(TeamLeader.User))]
    public ICollection<TeamLeader> TeamLeadership { get; set; } = [];

    [InverseProperty(nameof(MailingProfile.Owner))]
    public ICollection<MailingProfile> OwnedMailingFilters { get; set; } = [];

    [InverseProperty(nameof(MailingFilterUser.User))]
    public ICollection<MailingFilterUser> MailingFilters { get; set; } = [];

    public string GetFullName()
    {
        return FirstName +
               $"{(LastName is not null
                   ? " " + LastName
                   : string.Empty)}";
    }
}