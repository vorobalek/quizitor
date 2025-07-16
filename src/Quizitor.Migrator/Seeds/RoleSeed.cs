using Quizitor.Data;
using Quizitor.Data.Entities;

namespace Quizitor.Migrator.Seeds;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class RoleSeed(
    IDbContextProvider dbContextProvider) : ISeed
{
    public async Task ApplyAsync(CancellationToken cancellationToken)
    {
        Dictionary<string, string[]> rolePermissions = new()
        {
            {
                Role.SystemAdmin, [
                    UserPermission.BackOfficeMain,
                    UserPermission.BackOfficeBotList,
                    UserPermission.BackOfficeBotView,
                    UserPermission.BackOfficeBotEdit,
                    UserPermission.BackOfficeBotStart,
                    UserPermission.BackOfficeBotStop,
                    UserPermission.BackOfficeUserList,
                    UserPermission.BackOfficeUserView,
                    UserPermission.BackOfficeUserEdit,
                    UserPermission.BackOfficeGameList,
                    UserPermission.BackOfficeGameCreate,
                    UserPermission.BackOfficeGameDelete,
                    UserPermission.BackOfficeGameView,
                    UserPermission.BackOfficeRoundList,
                    UserPermission.BackOfficeRoundCreate,
                    UserPermission.BackOfficeRoundView,
                    UserPermission.BackOfficeQuestionList,
                    UserPermission.BackOfficeQuestionCreate,
                    UserPermission.BackOfficeQuestionView,
                    UserPermission.BackOfficeSessionList,
                    UserPermission.BackOfficeSessionCreate,
                    UserPermission.BackOfficeSessionEdit,
                    UserPermission.BackOfficeSessionView,
                    UserPermission.BackOfficeMailingList,
                    UserPermission.BackOfficeMailingCreate,
                    UserPermission.BackOfficeMailingView,
                    UserPermission.BackOfficeMailingSend,
                    UserPermission.BackOfficeServiceView,
                    UserPermission.BackOfficeUnlinkUserSessions
                ]
            },
            {
                Role.GameAdmin, [
                    UserPermission.LoadBalancerGameAdminAssign,
                    UserPermission.GameAdminMain,
                    UserPermission.GameAdminRoundList,
                    UserPermission.GameAdminRoundView,
                    UserPermission.GameAdminQuestionList,
                    UserPermission.GameAdminQuestionView,
                    UserPermission.GameAdminQuestionStart,
                    UserPermission.GameAdminQuestionStop,
                    UserPermission.GameAdminRatingStageShortView,
                    UserPermission.GameAdminRatingStageFullView,
                    UserPermission.GameAdminGameView,
                    UserPermission.GameAdminSessionLeave
                ]
            }
        };

        foreach (var (roleName, permissions) in rolePermissions)
            await ApplyRoleAsync(roleName, permissions, cancellationToken);
    }

    private async Task ApplyRoleAsync(
        string roleName,
        string[] permissions,
        CancellationToken cancellationToken)
    {
        var role = await dbContextProvider
            .Roles
            .GetBySystemNameOrDefaultAsync(
                roleName,
                cancellationToken);

        if (role is null)
        {
            role = new Role
            {
                SystemName = roleName
            };
            await dbContextProvider
                .Roles
                .AddAsync(
                    role,
                    cancellationToken);
        }

        var rolePermissions = await dbContextProvider
            .Roles
            .GetPermissionsByRoleIdAsync(
                role.Id,
                cancellationToken);

        await dbContextProvider
            .Roles
            .AddPermissionsAsync(
                permissions
                    .Except(rolePermissions.Select(x => x.SystemName))
                    .Select(x => new RolePermission
                    {
                        SystemName = x,
                        Role = role
                    }),
                cancellationToken);

        await dbContextProvider
            .Roles
            .RemovePermissionsAsync(
                rolePermissions.Where(x => !permissions.Contains(x.SystemName)),
                cancellationToken);
    }
}