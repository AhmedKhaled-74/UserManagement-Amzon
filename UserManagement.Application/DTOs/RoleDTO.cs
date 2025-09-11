using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Application.DTOs
{
    public class RoleDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public List<PermissionDTO>? Permissions { get; set; } = new();

    }
}