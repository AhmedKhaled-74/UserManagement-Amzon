using UserManagement.Domain.Entities.Identity;

namespace UserManagement.Application.DTOs.Mappers.UserMappers
{
    public static class PermissionMapper
    {
        public static PermissionDTO ToPermissionDTO(this Permission permission)
        {
            return new PermissionDTO
            {
                Id = permission.PermissionId,
                Task = permission.Task,
                Description = permission.Description
            };
        }

        public static Permission ToPermissionEntity(this PermissionDTO dto)
        {
            return new Permission
            {
                PermissionId = dto.Id,
                Task = dto.Task,
                Description = dto.Description
            };
        }
    }
}