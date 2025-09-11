using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Domain.Entities.Identity;

namespace UserManagement.Application.RepositoryContracts
{
    public interface IAdminRepository
    {
        // User Management
        Task DeactivateUserAsync(Guid userId);
        Task ActivateUserAsync(Guid userId);
        Task DeleteUserAsync(Guid userId);
        Task<IEnumerable<ApplicationUser>?> FilterUsersByRoleAsync(string roleName);
        Task<ApplicationUser?> GetUserDetailsAsync(Guid userId);
        Task AssignRoleAsync(Guid userId, string roleName);
        Task RevokeRoleAsync(Guid userId, string roleName);
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();

        // Permission Management
        Task<IEnumerable<Permission>> GetAllPermissionsAsync();
        Task<Permission?> GetPermissionByIdAsync(Guid permissionId);
        Task AddPermissionAsync(Permission permission);
        Task UpdatePermissionAsync(Permission permission);
        Task DeletePermissionAsync(Guid permissionId);

        // Role Management
        Task<IEnumerable<ApplicationRole>> GetAllRolesAsync();
        Task<ApplicationRole?> GetRoleByIdAsync(Guid roleId);
        Task AddRoleAsync(ApplicationRole role);
        Task UpdateRoleAsync(ApplicationRole role);
        Task DeleteRoleAsync(Guid roleId);
        Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId);
        Task RevokePermissionFromRoleAsync(Guid roleId, Guid permissionId);
    }
}
