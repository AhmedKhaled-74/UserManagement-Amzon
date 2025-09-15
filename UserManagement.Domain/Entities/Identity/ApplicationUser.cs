using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace UserManagement.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Region { get; set; }
        public string? ActiveStatus { get; set; }
        public string? RoleName { get; set; } = "User";

        public Guid RoleId { get; set; }

        // Navigation properties
        public virtual Guid? AdminId { get; set; }
        public virtual ApplicationUser? Admin { get; set; }
        public virtual ICollection<ApplicationUser> ManagedUsers { get; set; } = new List<ApplicationUser>();
        public virtual ApplicationRole Role { get; set; } = null!;
        public virtual ICollection<UserAddress?> Addresses { get; set; } = new List<UserAddress?>();
        public virtual ICollection<UserPhone?> Phones { get; set; } = new List<UserPhone?>();

        // Authentication
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }

    }
}