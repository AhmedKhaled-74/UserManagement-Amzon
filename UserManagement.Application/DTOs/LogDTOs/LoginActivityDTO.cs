using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Enums;

namespace UserManagement.Application.DTOs.LogDTOs
{
    public class LoginActivityDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string IpAddress { get; set; } =null!;
        public string Attempt { get; set; } = null!;
        public DateTime Timestamp { get; set; } 
    }
}
