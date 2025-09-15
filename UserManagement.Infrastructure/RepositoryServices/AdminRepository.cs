using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UserManagement.Application.RepositoryContracts;
using UserManagement.Domain.Entities.Identity;
using UserManagement.Infrastructure.DbContexts;

namespace UserManagement.Infrastructure.RepositoryServices
{
    public class AdminRepository : IAdminRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogRepo _logRepo;
        private readonly AppDbContext _db;

        public AdminRepository(UserManager<ApplicationUser> userManager,ILogRepo logRepo , RoleManager<ApplicationRole> roleManager, AppDbContext db)
        {
            _userManager = userManager;
            _logRepo = logRepo;
            _roleManager = roleManager;
            _db = db;
        }

        // User Management
        #region User Management
        public async Task DeactivateUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) throw new InvalidOperationException("User not found.");
            if (user.ActiveStatus == "Active")
            {
                user.ActiveStatus = "Inactive";
                user.RefreshToken = null;
                user.RefreshTokenExpiration = DateTime.UtcNow; // Invalidate refresh tokens
                await _userManager.UpdateAsync(user);
            }
            else
                throw new InvalidOperationException("already deactivated");
        }

        public async Task ActivateUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) throw new InvalidOperationException("User not found.");
            if(user.ActiveStatus == "Inactive")
            {
            user.ActiveStatus = "Active";
            await _userManager.UpdateAsync(user);
            }
            else
                throw new InvalidOperationException("already activated");
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) throw new InvalidOperationException("User not found.");
            if(user.RoleName == "Admin")
                throw new InvalidOperationException("Cannot delete Admin user.");
            if(user.ActiveStatus != "Suspend")
                throw new InvalidOperationException("Can only delete users with ' Suspend' status.");
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors));
        }

        public async Task<IEnumerable<ApplicationUser>?> FilterUsersByRoleAsync(string roleName)
        {
            return await _db.Users.Where(u=>string.Equals(u.RoleName,roleName,StringComparison.InvariantCultureIgnoreCase)).ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserDetailsAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user;
        }

        public async Task AssignRoleAsync(Guid userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) throw new InvalidOperationException("Role not found.");
            if (user == null) throw new InvalidOperationException("User not found.");

            if (user.RoleName!.Equals(roleName, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Cannot assign to the User does have this role already.");

            user.RoleName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(roleName.ToLower());
            user.Role = role;
            user.RoleId = role.Id;
            await _db.SaveChangesAsync();
        }

        public async Task RevokeRoleAsync(Guid userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) throw new InvalidOperationException("Role not found.");
            if (user == null) throw new InvalidOperationException("User not found.");

            if (!user.RoleName!.Equals(roleName, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Cannot revoke from the User doesn't have this role already.");

            if (roleName.Equals("User", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Cannot revoke the User role.");

            user.RoleName = "User";
            user.Role = await _roleManager.FindByNameAsync("User");
            user.RoleId = role.Id;
            await _db.SaveChangesAsync();

        }


        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _db.Users.ToListAsync();
        }
        #endregion

        // Role Management
        #region Role Management
        public async Task<IEnumerable<ApplicationRole>> GetAllRolesAsync()
        {
            return await _db.Roles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission).ToListAsync();
        }

        public async Task<ApplicationRole?> GetRoleByIdAsync(Guid roleId)
        {
            return await _db.Roles.Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission).FirstOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task AddRoleAsync(ApplicationRole role)
        {
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors));
        }

        public async Task UpdateRoleAsync(ApplicationRole role)
        {
            var existing = await _roleManager.FindByIdAsync(role.Id.ToString());
            if (existing == null) throw new InvalidOperationException("Role not found.");

            var usersInRole = await _db.Users
                .Where(u=>u.RoleId == existing.Id)
                .ToListAsync();

            existing.Name = role.Name;

            foreach (var user in usersInRole)
            {
                user.RoleName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(role.Name!.ToLower());
                user.RoleId = role.Id;
                await _userManager.UpdateAsync(user);
            }
                var result = await _roleManager.UpdateAsync(existing);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors));
        }

        public async Task DeleteRoleAsync(Guid roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null) throw new InvalidOperationException("Role not found.");
            if(role.Name == "Admin")
                throw new InvalidOperationException("Cannot delete Admin role.");
            if(role.Name =="User")
                throw new InvalidOperationException("Cannot delete User role.");
            // Find default "User" role

            var defaultRole = await _roleManager.FindByNameAsync("User");
            if (defaultRole == null)
                throw new InvalidOperationException("Default 'User' role not found.");

            var usersInRole = await _db.Users
                .Where(u => u.RoleId == role.Id)
                .ToListAsync();

            foreach (var user in usersInRole)
            {
              await AssignRoleAsync(user.Id, "User");
              await _logRepo.LogActivityAsync(user.Id, $"Role '{role.Name}' deleted. Assigned to default 'User' role.");
            }
            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors));
        }

        public async Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId)
        {
            var role = await _db.Roles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null) throw new InvalidOperationException("Role not found.");
            if (role.RolePermissions.Any(rp => rp.PermissionId == permissionId))
                throw new InvalidOperationException("Permission already assigned to role.");

            role.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permissionId });

            await _db.SaveChangesAsync();
        }
        public async Task RevokePermissionFromRoleAsync(Guid roleId, Guid permissionId)
        {
            var role = await _db.Roles.Include(r => r.RolePermissions).FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null) throw new InvalidOperationException("Role not found.");
            var rolePermission = role.RolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
            if (rolePermission == null)
                throw new InvalidOperationException("Permission not assigned to role.");
            role.RolePermissions.Remove(rolePermission);
            await _db.SaveChangesAsync();
        }

        #endregion
        // Permission Management
        #region Permission Management
        public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
        {
            return await _db.Permissions.OrderBy(p => p.Task).ToListAsync();
        }

        public async Task<Permission?> GetPermissionByIdAsync(Guid permissionId)
        {
            return await _db.Permissions.FindAsync(permissionId);
        }

        public async Task AddPermissionAsync(Permission permission)
        {
            var existing = await _db.Permissions.FirstOrDefaultAsync(p => p.Task.ToLower() == permission.Task.ToLower());
            if (existing != null)
                throw new InvalidOperationException("Permission with the same task already exists.");
            _db.Permissions.Add(permission);
            await _db.SaveChangesAsync();
        }

        public async Task UpdatePermissionAsync(Permission permission)
        {
            var existing = await _db.Permissions.FindAsync(permission.PermissionId);
            if (existing == null) throw new InvalidOperationException("Permission not found.");
            existing.Task = permission.Task;
            existing.Description = permission.Description;
            await _db.SaveChangesAsync();
        }

        public async Task DeletePermissionAsync(Guid permissionId)
        {
            var permission = await _db.Permissions.FindAsync(permissionId);
            if (permission == null) throw new InvalidOperationException("Permission not found.");
            _db.Permissions.Remove(permission);
            await _db.SaveChangesAsync();
        }

        #endregion
    }
}
