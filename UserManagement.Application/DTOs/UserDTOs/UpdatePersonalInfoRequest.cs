using System.ComponentModel.DataAnnotations;

namespace UserManagement.Application.DTOs.UserDTOs
{
    // Request DTO for updating personal info
    public class UpdatePersonalInfoRequest
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;


        public string? Region { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Birthdate { get; set; }
    }
}