using UserManagement.Domain.Entities.Identity;
using System.Linq;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.DTOs.Mappers.UserMappers
{
    public static class RoleMapper
    {
        public static RoleDTO ToRoleDTO(this ApplicationRole role)
        {
            return new RoleDTO
            {
                Id = role.Id,
                Name = role.Name!,
                Permissions = role.RolePermissions?.Select(rp => rp.Permission.ToPermissionDTO()).ToList() 
            };
        }

        public static ApplicationRole ToRoleEntity(this RoleDTO dto)
        {
            return new ApplicationRole
            {
                Id = dto.Id,
                Name = dto.Name
            };
        }
    }
}