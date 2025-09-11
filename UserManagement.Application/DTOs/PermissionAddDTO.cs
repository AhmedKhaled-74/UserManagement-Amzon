using System.ComponentModel.DataAnnotations;
using UserManagement.Domain.Entities.Identity;

namespace UserManagement.Application.DTOs
{
    public class PermissionAddDTO
    {
        [Required]
        [StringLength(100)]
        public string Task { get; set; } = string.Empty;
        [Required]
        [StringLength(250)]
        public string Description { get; set; } = string.Empty;
        public Permission ToPermissionEntity()
        {
            return new Permission()
            {
                PermissionId = Guid.NewGuid(),
                Task = this.Task,
                Description = this.Description
            };
        }
    }
}