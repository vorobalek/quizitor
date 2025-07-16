using Quizitor.Data.Entities;

namespace Quizitor.Data.Repositories;

public interface IRoleRepository
{
    Task AddAsync(Role role, CancellationToken cancellationToken);
    Task<Role?> GetByIdOrDefaultAsync(int roleId, CancellationToken cancellationToken);
    Task<Role[]> GetByUserIdAsync(long userId, CancellationToken cancellationToken);
    Task<Role?> GetBySystemNameOrDefaultAsync(string roleName, CancellationToken cancellationToken);
    Task<Role[]> GetPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
    Task GrantByUserIdAsync(long userId, Role role, CancellationToken cancellationToken);
    Task RevokeByUserIdAsync(long userId, Role role, CancellationToken cancellationToken);
    Task AddPermissionsAsync(IEnumerable<RolePermission> rolePermissions, CancellationToken cancellationToken);
    Task RemovePermissionsAsync(IEnumerable<RolePermission> rolePermissions, CancellationToken cancellationToken);
    Task<RolePermission[]> GetPermissionsByRoleIdAsync(int roleId, CancellationToken cancellationToken);
}