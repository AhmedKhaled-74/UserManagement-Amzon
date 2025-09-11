using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Domain.Entities.Identity;

namespace UserManagement.Application.DTOs.Mappers.AdminMappers
{
    public static class UserForAdminMapper
    {
        public static UserForAdminDTO ToUserAdminDTO(this ApplicationUser user)
        {
            return new UserForAdminDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Region = user.Region,
                Birthdate = user.DateOfBirth, 
                ActiveStatus = user.ActiveStatus!, 
                EmailConfirmed = user.EmailConfirmed,
                Role = user.RoleName!           
            };
        }

        public static ApplicationUser ToUserEntity(this UserForAdminDTO dto)
        {
            return new ApplicationUser
            {
                Id = dto.Id,
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email,
                Region = dto.Region,
                DateOfBirth = dto.Birthdate,
                EmailConfirmed = dto.EmailConfirmed,
                ActiveStatus = dto.ActiveStatus
            };
        }
    }
}
