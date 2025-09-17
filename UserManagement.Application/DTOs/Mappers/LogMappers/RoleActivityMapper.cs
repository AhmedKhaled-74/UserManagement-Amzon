using UserManagement.Application.DTOs.LogDTOs;
using UserManagement.Domain.LogsEntities;

namespace UserManagement.Application.DTOs.Mappers.LogMappers
{
    public static class RoleActivityMapper
    {
        public static RoleActivityDTO ToRoleActivityDTO(this RoleActivity activity)
        {
            return new RoleActivityDTO
            {
                Id = activity.Id,
                RoleId = activity.RoleId,
                Action = activity.Action,
                Timestamp = activity.Timestamp
            };
        }
        public static RoleActivity ToRoleActivityEntity(this RoleActivityDTO dto)
        {
            return new RoleActivity
            {
                Id = dto.Id,
                RoleId = dto.RoleId,
                Action = dto.Action,
                Timestamp = dto.Timestamp
            };
        }
    }
}
