using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Entities.Identity;

namespace UserManagement.Infrastructure.DbContexts
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public virtual DbSet<UserAddress> Addresses { get; set; }
        public virtual DbSet<UserPhone> Phones { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<LoginActivity> LoginActivities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // user entities

            modelBuilder.Entity<UserAddress>(entity =>
            {
                entity.HasKey(a => a.AddressId);
                entity.Property(a => a.Street).HasMaxLength(100).IsRequired();
                entity.HasOne<ApplicationUser>()
                      .WithMany(u => u.Addresses)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserPhone>(entity =>
            {
                entity.HasKey(p => p.PhoneId);
                entity.Property(p => p.PhoneNumber).HasMaxLength(15).IsRequired();
                entity.HasOne<ApplicationUser>()
                      .WithMany(u => u.Phones)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // auth entites

            modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<ApplicationRole>()
                      .WithMany()
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(p => p.PermissionId);
                entity.Property(p => p.Task).HasMaxLength(100).IsRequired();
                entity.Property(p => p.Description).HasMaxLength(250);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });
                entity.HasOne(rp => rp.Role)
                      .WithMany(r => r.RolePermissions)
                      .HasForeignKey(rp => rp.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(rp => rp.Permission)
                      .WithMany(p => p.RolePermissions)
                      .HasForeignKey(rp => rp.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // logs entities 
            modelBuilder.Entity<LoginActivity>()
           .Property(l => l.Attempt)
           .HasConversion<string>();
        }
    }
}

