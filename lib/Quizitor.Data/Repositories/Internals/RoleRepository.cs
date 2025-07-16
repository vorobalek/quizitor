using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;

namespace Quizitor.Data.Repositories.Internals;

internal sealed class RoleRepository(
    ApplicationDbContext dbContext) : IRoleRepository
{
    public async Task AddAsync(Role role, CancellationToken cancellationToken)
    {
        await dbContext.Roles.AddAsync(role, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Role?> GetByIdOrDefaultAsync(
        int roleId,
        CancellationToken cancellationToken)
    {
        return await dbContext
            .Roles
            .SingleOrDefaultAsync(
                x => x.Id == roleId,
                cancellationToken);
    }

    public Task<Role[]> GetByUserIdAsync(long userId, CancellationToken cancellationToken)
    {
        return dbContext
            .Roles
            .Where(x => x.Users.Any(e => e.Id == userId))
            .ToArrayAsync(cancellationToken);
    }

    public Task<Role?> GetBySystemNameOrDefaultAsync(
        string roleName,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Roles
            .SingleOrDefaultAsync(
                x => x.SystemName == roleName,
                cancellationToken);
    }

    public Task<Role[]> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return dbContext
            .Roles
            .OrderBy(x => x.SystemName)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return dbContext
            .Roles
            .CountAsync(cancellationToken);
    }

    public async Task GrantByUserIdAsync(
        long userId,
        Role role,
        CancellationToken cancellationToken)
    {
        var user = await dbContext
            .Users
            .Include(x => x.Roles)
            .SingleAsync(
                x => x.Id == userId,
                cancellationToken);
        user.Roles.Add(role);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeByUserIdAsync(
        long userId,
        Role role,
        CancellationToken cancellationToken)
    {
        var user = await dbContext
            .Users
            .Include(x => x.Roles)
            .SingleAsync(
                x => x.Id == userId,
                cancellationToken);
        user.Roles.Remove(role);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task AddPermissionsAsync(
        IEnumerable<RolePermission> rolePermissions,
        CancellationToken cancellationToken)
    {
        dbContext.RolePermissions.AddRange(rolePermissions);
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task RemovePermissionsAsync(
        IEnumerable<RolePermission> rolePermissions,
        CancellationToken cancellationToken)
    {
        dbContext.RolePermissions.RemoveRange(rolePermissions);
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<RolePermission[]> GetPermissionsByRoleIdAsync(
        int roleId,
        CancellationToken cancellationToken)
    {
        return dbContext
            .RolePermissions
            .Where(x => x.RoleId == roleId)
            .ToArrayAsync(cancellationToken);
    }
}