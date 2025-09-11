using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.DTOs.UserDTOs
{
    public class UserForAdminDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Region { get; set; }
        public string ActiveStatus { get; set; } = null!;
        public DateTime? Birthdate { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Role { get; set; } = string.Empty;

    }
}
