using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.LogDTOs;
using UserManagement.Domain.LogsEntities;

namespace UserManagement.Application.DTOs.Mappers.LogMappers
{
    public static class UserActivityMapper
    {
        public static UserActivityDTO ToUserActivityDTO(this UserActivity activity)
        {
            return new UserActivityDTO
            {
                Id = activity.Id,
                UserId = activity.UserId,
                Action = activity.Action,
                Timestamp = activity.Timestamp
            };
        }
        public static UserActivity ToUserActivityEntity(this UserActivityDTO dto)
        {
            return new UserActivity
            {
                Id = dto.Id,
                UserId = dto.UserId,
                Action = dto.Action,
                Timestamp = dto.Timestamp
            };
        }
    }
}
