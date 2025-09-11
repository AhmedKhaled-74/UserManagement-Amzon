using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Application.DTOs;
using UserManagement.Application.DTOs.Mappers.AdminMappers;
using UserManagement.Application.DTOs.Mappers.UserMappers;
using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Application.RepositoryContracts;
using UserManagement.Application.ServiceContracts;
using UserManagement.Domain.Entities.Identity;

namespace UserManagement.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepo;

        public AdminService(IAdminRepository adminRepo)
        {
            _adminRepo = adminRepo;
        }

        // User Management
        #region User Management
        public async Task DeactivateUserAsync(Guid? userId)
        {
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            await _adminRepo.DeactivateUserAsync(userId.Value);
        }

        public async Task ActivateUserAsync(Guid? userId)
        {
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            await _adminRepo.ActivateUserAsync(userId.Value);
        }

        public async Task DeleteUserAsync(Guid? userId)
        {
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            await _adminRepo.DeleteUserAsync(userId.Value);
        }

        public async Task<IEnumerable<UserForAdminDTO>?> FilterUsersByRoleAsync(string? roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name cannot be null or empty.", nameof(roleName));
            return (await _adminRepo.FilterUsersByRoleAsync(roleName))?.Select(u => u.ToUserAdminDTO());
        }

        public async Task<UserForAdminDTO?> GetUserDetailsAsync(Guid? userId)
        {
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            return (await _adminRepo.GetUserDetailsAsync(userId.Value))?.ToUserAdminDTO();
        }

        public async Task AssignRoleAsync(Guid? userId, string? roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name cannot be null or empty.", nameof(roleName));
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            await _adminRepo.AssignRoleAsync(userId.Value, roleName);
        }

        public async Task RevokeRoleAsync(Guid? userId, string? roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name cannot be null or empty.", nameof(roleName));
            if (userId == null || userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            if (roleName.Equals("User", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Cannot revoke the User role.");

            await _adminRepo.RevokeRoleAsync(userId.Value, roleName);
        }

        public async Task<IEnumerable<UserForAdminDTO>> GetAllUsersAsync()
            => (await _adminRepo.GetAllUsersAsync()).Select(u => u.ToUserAdminDTO());
        #endregion


        // Role Management
        #region Role Management
        public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync()
            => (await _adminRepo.GetAllRolesAsync()).Select(r => r.ToRoleDTO());

        public async Task<RoleDTO?> GetRoleByIdAsync(Guid? roleId)
        {
            if (roleId == null || roleId.Value == Guid.Empty)
                throw new ArgumentException("Role ID cannot be null or empty.", nameof(roleId));
            var role = await _adminRepo.GetRoleByIdAsync(roleId.Value);
            return role?.ToRoleDTO();
        }


        public async Task AddRoleAsync(RoleDTO? role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role), "Role cannot be null.");
            if (string.IsNullOrWhiteSpace(role.Name))
                throw new ArgumentException("Role name cannot be null or empty.", nameof(role.Name));
            var existingRoles = await _adminRepo.GetAllRolesAsync();
            if (existingRoles.Any(r => r.Name!.Equals(role.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Role '{role.Name}' already exists.");
            await _adminRepo.AddRoleAsync(role.ToRoleEntity());
        }

        public async Task UpdateRoleAsync(RoleDTO? role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role), "Role cannot be null.");
            await _adminRepo.UpdateRoleAsync(role.ToRoleEntity());
        }

        public async Task DeleteRoleAsync(Guid? roleId)
        {
            if (roleId == null || roleId.Value == Guid.Empty)
                throw new ArgumentException("Role ID cannot be null or empty.", nameof(roleId));
            await _adminRepo.DeleteRoleAsync(roleId.Value);
        }


        public async Task AssignPermissionToRoleAsync(Guid? roleId, Guid? permissionId)
        {
            if (roleId == null || roleId.Value == Guid.Empty)
                throw new ArgumentException("Role ID cannot be null or empty.", nameof(roleId));
            if (permissionId == null || permissionId.Value == Guid.Empty)
                throw new ArgumentException("Permission ID cannot be null or empty.", nameof(permissionId));
            await _adminRepo.AssignPermissionToRoleAsync(roleId.Value, permissionId.Value);
        }
        public async Task RevokePermissionFromRoleAsync(Guid? roleId, Guid? permissionId)
        {
            if (roleId == null || roleId.Value == Guid.Empty)
                throw new ArgumentException("Role ID cannot be null or empty.", nameof(roleId));
            if (permissionId == null || permissionId.Value == Guid.Empty)
                throw new ArgumentException("Permission ID cannot be null or empty.", nameof(permissionId));
            await _adminRepo.RevokePermissionFromRoleAsync(roleId.Value, permissionId.Value);
        }
        #endregion

        // Permission Management
        #region Permission Management
        public async Task<IEnumerable<PermissionDTO>> GetAllPermissionsAsync()
            => (await _adminRepo.GetAllPermissionsAsync()).Select(p => p.ToPermissionDTO());

        public async Task<PermissionDTO?> GetPermissionByIdAsync(Guid? permissionId)
        {
            if (permissionId == null || permissionId.Value == Guid.Empty)
                throw new ArgumentException("Permission ID cannot be null or empty.", nameof(permissionId));
            var permission = await _adminRepo.GetPermissionByIdAsync(permissionId.Value);
            return permission?.ToPermissionDTO();
        }

        public async Task AddPermissionAsync(PermissionAddDTO? permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission), "Permission cannot be null.");
            await _adminRepo.AddPermissionAsync(permission.ToPermissionEntity());
        }


        public async Task UpdatePermissionAsync(PermissionDTO? permission)
        {
            if (permission == null)
                throw new ArgumentNullException(nameof(permission), "Permission cannot be null.");
            await _adminRepo.UpdatePermissionAsync(permission.ToPermissionEntity());
        }

        public async Task DeletePermissionAsync(Guid? permissionId)
        {
            if (permissionId == null || permissionId.Value == Guid.Empty)
                throw new ArgumentException("Permission ID cannot be null or empty.", nameof(permissionId));
            await _adminRepo.DeletePermissionAsync(permissionId.Value);
        }
        #endregion


    }
}
