using System.ComponentModel.DataAnnotations;

namespace UserManagement.Application.DTOs.UserDTOs
{
    public class ResetPassRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

    }
}
