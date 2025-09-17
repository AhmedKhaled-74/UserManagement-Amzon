using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Enums;
using UserManagement.Domain.LogsEntities;

namespace UserManagement.Application.RepositoryContracts
{
    public interface ILogRepo
    {
        Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(Guid userId);
        Task<IEnumerable<UserActivity>> GetAllUsersActivitiesAsync();
        Task<IEnumerable<LoginActivity>> GetAllUsersLoginActivitiesAsync();
        Task<IEnumerable<LoginActivity>?> GetUserLoginActivitiesAsync(Guid userId);

        Task<IEnumerable<RoleActivity>?> GetAllRolesActivitiesAsync();


        Task LogActivityAsync(Guid userId, string action);
        Task LogLoginActivityAsync(Guid userId, LoginAttempts attempt, string? Ip);
        Task LogRoleActivityAsync(Guid roleId, string action);

    }
}
