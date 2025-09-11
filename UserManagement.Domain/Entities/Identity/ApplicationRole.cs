using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace UserManagement.Domain.Entities.Identity
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}