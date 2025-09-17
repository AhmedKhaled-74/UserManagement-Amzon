using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Enums;
using UserManagement.Application.RepositoryContracts;
using UserManagement.Infrastructure.DbContexts;
using UserManagement.Domain.LogsEntities;

namespace UserManagement.Infrastructure.RepositoryServices
{
    public class LogRepo : ILogRepo
    {
        private readonly AppDbContext _context;
        public LogRepo(AppDbContext context) 
        {
            _context = context;
        }
        public async Task<IEnumerable<UserActivity>> GetAllUsersActivitiesAsync()
        {
           return await _context.UserActivities.OrderByDescending(ua=>ua.Timestamp).ToListAsync();
        }

        public async Task<IEnumerable<LoginActivity>> GetAllUsersLoginActivitiesAsync()
        {
            return await _context.LoginActivities.OrderByDescending(la => la.Timestamp).ToListAsync();
        }


        public async Task<IEnumerable<RoleActivity>?> GetAllRolesActivitiesAsync()
        {
            return await _context.RoleActivities.OrderByDescending(la => la.Timestamp).ToListAsync();
        }



        public async Task LogActivityAsync(Guid userId, string action)
        {
            await _context.UserActivities.AddAsync(new UserActivity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Action = action,
                Timestamp = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        public async Task LogLoginActivityAsync(Guid userId, LoginAttempts attempt, string? Ip)
        {
            await _context.LoginActivities.AddAsync(new LoginActivity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                IpAddress = Ip??"",
                Attempt = attempt.ToString(),
                Timestamp = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        public async Task LogRoleActivityAsync(Guid roleId , string action)
        {
            await _context.RoleActivities.AddAsync(new RoleActivity
            {
                Id = Guid.NewGuid(),
                RoleId = roleId,
                Action = action ?? "",
                Timestamp = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserActivity>> GetUserActivitiesAsync(Guid userId)
        {
          return  await _context.UserActivities.Where(ua => ua.UserId == userId).OrderByDescending(ua => ua.Timestamp).ToListAsync();
        }

        public async Task<IEnumerable<LoginActivity>?> GetUserLoginActivitiesAsync(Guid userId)
        {
           return await _context.LoginActivities.Where(la => la.UserId == userId).OrderByDescending(la => la.Timestamp).ToListAsync();
        }
    }
}
