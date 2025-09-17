using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.LogDTOs;
using UserManagement.Domain.LogsEntities;

namespace UserManagement.Application.DTOs.Mappers.LogMappers
{
    public static class LoginActivityMapper
    {
        public static LoginActivityDTO ToLoginActivityDTO(this LoginActivity activity)
        {
            return new LoginActivityDTO
            {
                Id = activity.Id,
                UserId = activity.UserId,
                IpAddress = activity.IpAddress,
                Attempt = activity.Attempt,
                Timestamp = activity.Timestamp
            };
        }
        public static LoginActivity ToLoginActivityEntity(this LoginActivityDTO dto)
        {
            return new LoginActivity
            {
                Id = dto.Id,
                UserId = dto.UserId,
                IpAddress = dto.IpAddress,
                Attempt = dto.Attempt,
                Timestamp = dto.Timestamp
            };
        }
    }
}
