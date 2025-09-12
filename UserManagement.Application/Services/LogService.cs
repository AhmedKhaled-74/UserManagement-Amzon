using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Enums;
using UserManagement.Application.RepositoryContracts;
using UserManagement.Application.ServiceContracts;
using UserManagement.Application.DTOs;
using UserManagement.Application.DTOs.Mappers.LogMappers;

namespace UserManagement.Application.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepo _logRepo;
        public LogService(ILogRepo logRepo)
        {
            _logRepo = logRepo;
        }
        public async Task<IEnumerable<UserActivityDTO>> GetAllUsersActivitiesAsync()
        {
            var userActivites= await _logRepo.GetAllUsersActivitiesAsync();
            return userActivites.Select(ua => ua.ToUserActivityDTO());
        }

        public async Task<IEnumerable<LoginActivityDTO>> GetAllUsersLoginActivitiesAsync()
        {
            var loginActivities =  await _logRepo.GetAllUsersLoginActivitiesAsync();
            return  loginActivities.Select(la => la.ToLoginActivityDTO());
        }

        public async Task LogActivityAsync(Guid? userId, string? action)
        {
            if (userId == null || action == null)
                throw new ArgumentNullException("UserId or Action cannot be null");

             await _logRepo.LogActivityAsync(userId.Value, action);  
        }

        public async Task LogLoginActivityAsync(Guid? userId, LoginAttempts? attempt,string? Ip)
        {
            if (userId == null || attempt == null)
                throw new ArgumentNullException("UserId or Attempt cannot be null");
            await _logRepo.LogLoginActivityAsync(userId.Value, attempt.Value ,Ip);
        }

        public async Task<IEnumerable<UserActivityDTO>> GetUserActivitiesAsync(Guid? userId)
        {
            if (userId == null)
                throw new ArgumentNullException("UserId cannot be null");
            var userActivities = await _logRepo.GetUserActivitiesAsync(userId.Value);
            return userActivities.Select(ua => ua.ToUserActivityDTO());
        }

        public async Task<IEnumerable<LoginActivityDTO>?> GetUserLoginActivitiesAsync(Guid? userId)
        {
            if (userId == null)
                throw new ArgumentNullException("UserId cannot be null");
            var loginActivities = await _logRepo.GetUserLoginActivitiesAsync(userId.Value);
            return loginActivities?.Select(la => la.ToLoginActivityDTO());
        }
    }
}
