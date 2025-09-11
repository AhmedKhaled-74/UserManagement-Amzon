using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Application.DTOs;


namespace UserManagement.Application.ServiceContracts
{
    public interface IAdminService
    {
        // User Management
        Task DeactivateUserAsync(Guid? userId);
        Task ActivateUserAsync(Guid? userId);
        Task DeleteUserAsync(Guid? userId);
        Task<IEnumerable<UserForAdminDTO>?> FilterUsersByRoleAsync(string? roleName);
        Task<UserForAdminDTO?> GetUserDetailsAsync(Guid? userId);
        Task AssignRoleAsync(Guid? userId, string? roleName);
        Task RevokeRoleAsync(Guid? userId, string? roleName);
        Task<IEnumerable<UserForAdminDTO>> GetAllUsersAsync();

        // Permission Management
        Task<IEnumerable<PermissionDTO>> GetAllPermissionsAsync();
        Task<PermissionDTO?> GetPermissionByIdAsync(Guid? permissionId);
        Task AddPermissionAsync(PermissionAddDTO? permission);
        Task UpdatePermissionAsync(PermissionDTO? permission);
        Task DeletePermissionAsync(Guid? permissionId);

        // Role Management
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO?> GetRoleByIdAsync(Guid? roleId);
        Task AddRoleAsync(RoleDTO? role);
        Task UpdateRoleAsync(RoleDTO? role);
        Task DeleteRoleAsync(Guid? roleId);
        Task AssignPermissionToRoleAsync(Guid? roleId, Guid? permissionId);
        Task RevokePermissionFromRoleAsync(Guid? roleId, Guid? permissionId);
    }
}
