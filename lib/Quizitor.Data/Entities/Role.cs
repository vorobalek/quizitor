using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("role", Schema = "public")]
[PrimaryKey(nameof(Id))]
[Index(nameof(SystemName), IsUnique = true)]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Role
{
    public const string SystemAdmin = "SYSADMIN";
    public const string GameAdmin = "GAMEADMIN";

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(256)]
    public string SystemName { get; set; } = null!;

    [InverseProperty(nameof(RolePermission.Role))]
    public ICollection<RolePermission> Permissions { get; set; } = [];

    [InverseProperty(nameof(User.Roles))]
    public ICollection<User> Users { get; set; } = [];
}