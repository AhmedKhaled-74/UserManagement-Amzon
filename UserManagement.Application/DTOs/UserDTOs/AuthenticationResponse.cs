using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.DTOs.UserDTOs
{
    public class AuthenticationResponse
    {
        public string? PersonName { get; set; }
        public string? UserName { get; set; } 
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? RoleName { get; set; }
        public DateTime? RefreshTokenExpirationDate { get; set; }
        public DateTime Expiration { get; set; }

    }
}
