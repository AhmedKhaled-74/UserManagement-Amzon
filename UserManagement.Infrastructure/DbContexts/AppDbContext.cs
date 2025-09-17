using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Entities.Identity;
using UserManagement.Domain.LogsEntities;

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
        public DbSet<RoleActivity> RoleActivities { get; set; }
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

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Role)          // one role per user
                .WithMany(r => r.Users)       // a role can have many users
                .HasForeignKey(u => u.RoleId) // FK in ApplicationUser
                .OnDelete(DeleteBehavior.Restrict);
            // auth entites

            

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

            modelBuilder.Entity<LoginActivity>(entity =>
            { 
                entity.HasKey(la => la.Id);
                entity.Property(la => la.Attempt).HasConversion<string>();
            });

            modelBuilder.Entity<UserActivity>(entity =>
            {
                entity.HasKey(ua => ua.Id);
                entity.Property(ua => ua.Action).HasMaxLength(255).IsRequired();
                
            });

            modelBuilder.Entity<RoleActivity>(entity =>
            {  
                entity.HasKey(ra => ra.Id);
                entity.Property(ra => ra.Action).HasMaxLength(255).IsRequired();

            });




        }
    }
}

