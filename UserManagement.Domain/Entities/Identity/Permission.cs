using System;

namespace UserManagement.Domain.Entities.Identity
{
    public class Permission
    {
        public Guid PermissionId { get; set; }
        public string Task { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
   
}