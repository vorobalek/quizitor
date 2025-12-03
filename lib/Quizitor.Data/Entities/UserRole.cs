using System.ComponentModel.DataAnnotations.Schema;

namespace Quizitor.Data.Entities;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
[Table("user_role", Schema = "public")]
public class UserRole
{
    public long UserId { get; set; }

    public int RoleId { get; set; }
}