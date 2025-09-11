using System;
using System.ComponentModel.DataAnnotations;
using UserManagement.Domain.Entities.Identity;

namespace UserManagement.Application.DTOs
{
    public class PermissionDTO
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Task { get; set; } = string.Empty;
        [Required]
        [StringLength(250)]
        public string Description { get; set; } = string.Empty;

        public Permission ToPermissionEntity() => new Permission
        {
            PermissionId = this.Id,
            Task = this.Task,
            Description = this.Description
        };
    }
}