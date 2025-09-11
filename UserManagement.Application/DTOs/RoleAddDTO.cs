using System.ComponentModel.DataAnnotations;

namespace UserManagement.Application.DTOs
{
    public class RoleAddDTO
    {

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}