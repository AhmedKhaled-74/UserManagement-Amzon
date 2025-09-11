using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Application.DTOs.UserDTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Region { get; set; }
        public DateTime? Birthdate { get; set; }
    }
}