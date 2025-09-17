using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.LogDTOs;
using UserManagement.Domain.Enums;
using UserManagement.Domain.LogsEntities;

namespace UserManagement.Application.ServiceContracts
{
    public interface ILogService
    {
        Task<IEnumerable<UserActivityDTO>> GetUserActivitiesAsync(Guid? userId);
        Task<IEnumerable<UserActivityDTO>> GetAllUsersActivitiesAsync();
        Task<IEnumerable<LoginActivityDTO>> GetAllUsersLoginActivitiesAsync();
        Task<IEnumerable<RoleActivityDTO>?> GetAllRolesActivitiesAsync();
        Task<IEnumerable<LoginActivityDTO>?> GetUserLoginActivitiesAsync(Guid? userId);

        Task LogActivityAsync(Guid? userId, string? action);
        Task LogLoginActivityAsync(Guid? userId, LoginAttempts? attempt , string? Ip);
        Task LogRoleActivityAsync(Guid? RoleId, string? action);
    }
}
