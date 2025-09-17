using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using UserManagement.Application.DTOs;
using UserManagement.Application.DTOs.LogDTOs;
using UserManagement.Application.DTOs.Mappers.UserMappers;
using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Application.ServiceContracts;

namespace UserManagement.Presentation.Controllers.v1
{
    /// <summary>
    /// administrative operations for user, role, and permission management
    /// </summary>
    [Authorize(Roles = "Admin")]
    // [AllowAnonymous]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogService _logService;

        private readonly ILogger<AdminController> _logger;
        /// <summary>
        /// constructor with DI for admin service and logger
        /// </summary>
        /// <param name="adminService"></param>
       
        /// <param name="logger"></param>
        public AdminController(IAdminService adminService , ILogService logService , ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logService = logService;
            _logger = logger;
        }

        // ---------------------- USER MANAGEMENT ----------------------
        #region User Management
        /// <summary>Get all users</summary>
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserForAdminDTO>>> GetAllUsers()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();

               
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>Get user details</summary>
        [HttpGet("users/{userId:guid}")]
        public async Task<ActionResult<UserForAdminDTO>> GetUserDetails(Guid userId)
        {
            try
            {
                var user = await _adminService.GetUserDetailsAsync(userId);
                if (user == null)
                    return NotFound($"User with ID {userId} not found");
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user details {UserId}", userId);
                return ExceptionHandel(ex);
            }
        }

        /// <summary>Get users by role</summary>
        [HttpGet("users/by-role/{roleName}")]
        public async Task<ActionResult<IEnumerable<UserForAdminDTO>>> FilterUsersByRole(string roleName)
        {
            try
            {
                var users = await _adminService.FilterUsersByRoleAsync(roleName);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering users by role {Role}", roleName);
                return ExceptionHandel(ex);
            }
        }

        /// <summary>Deactivate user</summary>
        [HttpPatch("users/{userId:guid}/deactivate")]
        public async Task<IActionResult> DeactivateUser(Guid userId)
        {
            try
            {
                await _adminService.DeactivateUserAsync(userId);
                await _logService.LogActivityAsync(userId, "User Deactivated by Admin");
                return Ok(new { message = "User deactivated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user {UserId}", userId);
                await _logService.LogActivityAsync(userId, $"Error Deactivating User: {ex.Message}");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>Activate user</summary>
        [HttpPatch("users/{userId:guid}/activate")]
        public async Task<IActionResult> ActivateUser(Guid userId)
        {
            try
            {
                await _adminService.ActivateUserAsync(userId);
                await _logService.LogActivityAsync(userId, "User Activated by Admin");
                return Ok(new { message = "User activated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user {UserId}", userId);
                await _logService.LogActivityAsync(userId, $"Error Activating User: {ex.Message}");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>Delete user</summary>
         [HttpDelete("users/{userId:guid}")]
         public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                await _adminService.DeleteUserAsync(userId);
                await _logService.LogActivityAsync(userId, "User Deleted by Admin");
                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                await _logService.LogActivityAsync(userId, $"Error Deleting User: {ex.Message}");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>Assign role to user</summary>
        [HttpPost("users/{userId:guid}/roles/{roleName}")]
        public async Task<IActionResult> AssignRole(Guid userId, string roleName)
        {
            try
            {
                await _adminService.AssignRoleAsync(userId, roleName);
                await _logService.LogActivityAsync(userId, $"Role '{roleName}' assigned by Admin");
                return Ok(new { message = $"Role '{roleName}' assigned successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {Role} to user {UserId}", roleName, userId);
                await _logService.LogActivityAsync(userId, $"Error Assigning Role '{roleName}': {ex.Message}");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>Revoke role from user</summary>
        [HttpDelete("users/{userId:guid}/roles/{roleName}")]
        public async Task<IActionResult> RevokeRole(Guid userId, string roleName)
        {
            try
            {
                await _adminService.RevokeRoleAsync(userId, roleName);
                await _logService.LogActivityAsync(userId, $"Role '{roleName}' revoked by Admin");
                return Ok(new { message = $"Role '{roleName}' revoked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking role {Role} from user {UserId}", roleName, userId);
                await _logService.LogActivityAsync(userId, $"Error Revoking Role '{roleName}': {ex.Message}");
                return ExceptionHandel(ex);
            }
        }
        #endregion

        // ---------------------- ROLE MANAGEMENT ----------------------
        #region Role Management

        /// <summary>Get all roles</summary>
        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetAllRoles()
        {
            try
            {
                var roles = await _adminService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>Get role by ID</summary>
        [HttpGet("roles/{roleId:guid}")]
        public async Task<ActionResult<RoleDTO>> GetRoleById(Guid roleId)
        {
            try
            {
                var role = await _adminService.GetRoleByIdAsync(roleId);
                if (role == null)
                    return NotFound($"Role with ID {roleId} not found");
                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role {RoleId}", roleId);
               return ExceptionHandel(ex);
            }
        }

        /// <summary>Add role</summary>
        [HttpPost("roles")]
        public async Task<IActionResult> AddRole([FromBody] RoleAddDTO role)
        {
            try
            {
                var newRole =new RoleDTO
                {
                    Id = Guid.NewGuid(),
                    Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(role.Name!.ToLower())
                };
                await _adminService.AddRoleAsync(newRole);
                await _logService.LogRoleActivityAsync(newRole.Id, $"Role with name {role.Name} added successfully");
                return Ok(new { message = "Role added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding role");
                return StatusCode(500, ex.Message??"Internal server error");
            }
        }

        /// <summary>Update role</summary>
        [HttpPut("roles/{roleId:guid}")]
        public async Task<IActionResult> UpdateRole(Guid roleId, [FromBody] RoleDTO role)
        {
            try
            {
                if (roleId != role.Id)
                    return BadRequest("Role ID mismatch");
                var oldRole =  await _adminService.GetRoleByIdAsync(roleId); 
                if (oldRole == null)
                    return BadRequest("Role ID NotFound");
                await _adminService.UpdateRoleAsync(role);
                await _logService.LogRoleActivityAsync(role.Id, $"Role with old name {oldRole?.Name} Updated with {role.Name} successfully");

                return Ok(new { message = "Role updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {RoleId}", roleId);
                return ExceptionHandel(ex);
            }
        }

        /// <summary>Delete role</summary>
        [HttpDelete("roles/{roleId:guid}")]
        public async Task<IActionResult> DeleteRole(Guid roleId)
        {
            try
            {
                var role = await _adminService.GetRoleByIdAsync(roleId);
                if (role == null)
                    return NotFound();
                await _adminService.DeleteRoleAsync(roleId);
                await _logService.LogRoleActivityAsync(roleId, $"Role with name {role.Name} Deleted successfully");
                return Ok(new { message = "Role deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {RoleId}", roleId);
                return ExceptionHandel(ex);
            }
        }

        /// <summary>Assign permission to role</summary>
        [HttpPost("roles/{roleId:guid}/permissions/{permissionId:guid}")]
        public async Task<IActionResult> AssignPermissionToRole(Guid roleId, Guid permissionId)
        {
            try
            {
                var role = await _adminService.GetRoleByIdAsync(roleId);
                var permission = await _adminService.GetPermissionByIdAsync(permissionId);
                if (role == null || permission == null)
                    return NotFound();
                await _adminService.AssignPermissionToRoleAsync(roleId, permissionId);
                await _logService.LogRoleActivityAsync(roleId, $"Role with name {role.Name} added new persission with Task {permission.Task} successfully");

                return Ok(new { message = "Permission assigned to role successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permission {PermissionId} to role {RoleId}", permissionId, roleId);
                return ExceptionHandel(ex);
            }
        }

        /// <summary>Revoke permission from role</summary>
        [HttpDelete("roles/{roleId:guid}/permissions/{permissionId:guid}")]
        public async Task<IActionResult> RevokePermissionFromRole(Guid roleId, Guid permissionId)
        {
            try
            {
                var role = await _adminService.GetRoleByIdAsync(roleId);
                var permission = await _adminService.GetPermissionByIdAsync(permissionId);
                if (role == null || permission == null)
                    return NotFound();
                await _adminService.RevokePermissionFromRoleAsync(roleId, permissionId);
                await _logService.LogRoleActivityAsync(roleId, $"Role with name {role.Name} revoked new persission with Task {permission.Task} successfully");
                return Ok(new { message = "Permission revoked from role successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking permission {PermissionId} from role {RoleId}", permissionId, roleId);
                await _logService.LogRoleActivityAsync(roleId, $"Role revoked a persission unsuccessfully");

                return ExceptionHandel(ex);
            }
        }
        #endregion

        // ---------------------- PERMISSION MANAGEMENT ----------------------
        #region Permission Management
        /// <summary>Get all permissions</summary>
        [HttpGet("permissions")]
        public async Task<ActionResult<IEnumerable<PermissionDTO>>> GetAllPermissions()
        {
            try
            {
                var permissions = await _adminService.GetAllPermissionsAsync();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all permissions");
               return ExceptionHandel(ex);
            }
        }

        /// <summary>Get permission by ID</summary>
        [HttpGet("permissions/{permissionId:guid}")]
        public async Task<ActionResult<PermissionDTO>> GetPermissionById(Guid permissionId)
        {
            try
            {
                var permission = await _adminService.GetPermissionByIdAsync(permissionId);
                if (permission == null)
                    return NotFound($"Permission with ID {permissionId} not found");
                return Ok(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permission {PermissionId}", permissionId);
               return ExceptionHandel(ex);
            }
        }

        /// <summary>Add permission</summary>
        [HttpPost("permissions")]
        public async Task<IActionResult> AddPermission([FromBody] PermissionAddDTO permission)
        {
            try
            {
                await _adminService.AddPermissionAsync(permission);
                return Ok(new { message = "Permission added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding permission");
                return ExceptionHandel(ex);
            }
        }

        /// <summary>Update permission</summary>
        [HttpPut("permissions/{permissionId:guid}")]
        public async Task<IActionResult> UpdatePermission(Guid permissionId, [FromBody] PermissionDTO permission)
        {
            try
            {
                if (permissionId != permission.Id)
                    return BadRequest("Permission ID mismatch");

                await _adminService.UpdatePermissionAsync(permission);
                return Ok(new { message = "Permission updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permission {PermissionId}", permissionId);
               return ExceptionHandel(ex);
            }
        }

        /// <summary>Delete permission</summary>
        [HttpDelete("permissions/{permissionId:guid}")]
        public async Task<IActionResult> DeletePermission(Guid permissionId)
        {
            try
            {
                await _adminService.DeletePermissionAsync(permissionId);
                return Ok(new { message = "Permission deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting permission {PermissionId}", permissionId);
               return ExceptionHandel(ex);
            }
        }
        #endregion

        // ---------------------- Audit MANAGEMENT ----------------------
        #region Audit Management
        /// <summary>Get all user activities</summary>
        
        [HttpGet("logs/user-activities")]
        public async Task<ActionResult<IEnumerable<UserActivityDTO>>> GetAllUserActivities()
        {
            try
            {
                var logs = await _logService.GetAllUsersActivitiesAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all user activities");
               return ExceptionHandel(ex);
            }
        }

        /// <summary>Get all roles activities</summary>

        [HttpGet("logs/roles-activities")]
        public async Task<ActionResult<IEnumerable<UserActivityDTO>>> GetAllRolesActivities()
        {
            try
            {
                var logs = await _logService.GetAllRolesActivitiesAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles activities");
                return ExceptionHandel(ex);
            }
        }
        /// <summary>Get user activities by user ID</summary>

        [HttpGet("logs/user-activities/{userId:guid}")]
        public async Task<ActionResult<IEnumerable<UserActivityDTO>>> GetUserActivities(Guid userId)
        {
            try
            {
                var logs = await _logService.GetUserActivitiesAsync(userId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activities for user {UserId}", userId);
               return ExceptionHandel(ex);
            }
        }

        /// <summary>Get all login activities</summary>

        [HttpGet("logs/login-activities")]
        public async Task<ActionResult<IEnumerable<LoginActivityDTO>>> GetAllLoginActivities()
        {
            try
            {
                var logs = await _logService.GetAllUsersLoginActivitiesAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all login activities");
               return ExceptionHandel(ex);
            }
        }

        /// <summary>Get login activities by user ID</summary>
        
        [HttpGet("logs/login-activities/{userId:guid}")]
        public async Task<ActionResult<IEnumerable<LoginActivityDTO>>> GetUserLoginActivities(Guid userId)
        {
            try
            {
                var logs = await _logService.GetUserLoginActivitiesAsync(userId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting login activities for user {UserId}", userId);
               return ExceptionHandel(ex);
            }
        }

        #endregion

        // ---------------------- EXCEPTION HANDLER ----------------------
        /// <summary>
        /// handel exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private ActionResult ExceptionHandel(Exception ex) {
            if (ex.InnerException != null)
                return StatusCode(500, ex.InnerException.Message);
            if (!string.IsNullOrEmpty(ex.Message))
                return StatusCode(500, ex.Message);
            if (ex is ArgumentNullException || ex is ArgumentException || ex is InvalidOperationException)
                return BadRequest(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }
}
