using Domain.Entities;
using Domain.Entities.Junction;
using Domain.Entities.Masterlist;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<PendingAccount> PendingAccounts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Uom> Uoms { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<MiscellaneousReceipt> MiscellaneousReceipt { get; set; }
        public DbSet<WarehouseReceiving> WarehouseReceivings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.IsActive))
                    .HasDefaultValue(true);
            }

            foreach (var property in modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(e => e.GetProperties())
                .Where(p => p.ClrType == typeof(decimal)))
            {
                property.SetPrecision(18);
                property.SetScale(4);
            }

            modelBuilder.Entity<User>()
                .HasOne(u => u.CreatedBy)
                .WithMany()
                .HasForeignKey(u => u.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.UpdatedBy)
                .WithMany()
                .HasForeignKey(u => u.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PendingAccount>()
                .HasOne(f => f.CreatedBy)
                .WithMany()
                .HasForeignKey(f => f.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PendingAccount>()
                .HasOne(f => f.UpdatedBy)
                .WithMany()
                .HasForeignKey(f => f.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>()
                .HasOne(r => r.CreatedBy)
                .WithMany()
                .HasForeignKey(r => r.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>()
                .HasOne(r => r.UpdatedBy)
                .WithMany()
                .HasForeignKey(r => r.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Permission>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Permission>()
                .HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRoles>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRoles>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRoles>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermissions>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermissions>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermissions>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Uom)
                .WithMany()
                .HasForeignKey(p => p.UomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Uom>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Uom>()
                .HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Warehouse>()
                .HasOne(w => w.CreatedBy)
                .WithMany()
                .HasForeignKey(w => w.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Warehouse>()
                .HasOne(w => w.UpdatedBy)
                .WithMany()
                .HasForeignKey(w => w.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MiscellaneousReceipt>()
                .HasOne(d => d.CreatedBy)
                .WithMany()
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MiscellaneousReceipt>()
                .HasOne(d => d.UpdatedBy)
                .WithMany()
                .HasForeignKey(d => d.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MiscellaneousReceipt>()
                .HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WarehouseReceiving>()
                .HasOne(wr => wr.CreatedBy)
                .WithMany()
                .HasForeignKey(wr => wr.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WarehouseReceiving>()
                .HasOne(wr => wr.UpdatedBy)
                .WithMany()
                .HasForeignKey(wr => wr.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WarehouseReceiving>()
                .HasOne(wr => wr.Product)
                .WithMany()
                .HasForeignKey(wr => wr.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WarehouseReceiving>()
                .HasOne(wr => wr.MiscellaneousReceipt)
                .WithMany()
                .HasForeignKey(wr => wr.MiscellaneousReceiptId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Name = "Admin",
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "$2a$11$yN1InXU5MK7Q6oq1/N4MYume.CEqdgFpBAC9ffnk8nHZ9LybM9U0u",
                    FirstName = "",
                    MiddleName = null,
                    LastName = "",
                    Suffix = null,
                    IDPrefix = "",
                    IDNumber = "",
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                }
            );

            modelBuilder.Entity<UserRoles>().HasData(
                new UserRoles
                {
                    UserId = 1,
                    RoleId = 1
                }
            );
        }
    }
}
