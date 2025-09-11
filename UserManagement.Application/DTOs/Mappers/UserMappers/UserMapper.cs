using UserManagement.Application.DTOs.UserDTOs;
using UserManagement.Domain.Entities.Identity;

namespace UserManagement.Application.DTOs.Mappers.UserMappers
{
    public static class UserMapper
    {
        public static UserDTO ToUserDTO(this ApplicationUser user)
        {
            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Region = user.Region,
                Birthdate = user.DateOfBirth
            };
        }

        public static ApplicationUser ToUserEntity(this UserDTO dto)
        {
            return new ApplicationUser
            {
                Id = dto.Id,
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email,
                Region = dto.Region,
                DateOfBirth = dto.Birthdate
            };
        }
    }
}