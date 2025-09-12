using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Enums;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.ServiceContracts
{
    public interface ILogService
    {
        Task<IEnumerable<UserActivityDTO>> GetUserActivitiesAsync(Guid? userId);
        Task<IEnumerable<UserActivityDTO>> GetAllUsersActivitiesAsync();
        Task<IEnumerable<LoginActivityDTO>> GetAllUsersLoginActivitiesAsync();
        Task<IEnumerable<LoginActivityDTO>?> GetUserLoginActivitiesAsync(Guid? userId);

        Task LogActivityAsync(Guid? userId, string? action);
        Task LogLoginActivityAsync(Guid? userId, LoginAttempts? attempt , string? Ip);
    }
}
