using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quizitor.Data.Entities;

[Table("role_permission", Schema = "public")]
[PrimaryKey(nameof(Id))]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class RolePermission
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(Role))]
    public int RoleId { get; set; }

    [InverseProperty(nameof(Entities.Role.Permissions))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Role Role { get; set; } = null!;

    [MaxLength(256)]
    public string SystemName { get; set; } = null!;
}