using Domain.Entities;
using Domain.Entities.Junction;
using Domain.Entities.Masterlist;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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
        public DbSet<MiscellaneousReceipt> MiscellaneousReceipts { get; set; }
        public DbSet<WarehouseReceiving> WarehouseReceivings { get; set; }
        public DbSet<MoveOrder> MoveOrders { get; set; }
        public DbSet<MoveOrderProducts> MoveOrderProducts { get; set; }
        public DbSet<MoveOrderProductWarehouseReceivings> MoveOrderProductWarehouseReceivings { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 4);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes()
                         .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.IsActive))
                    .HasDefaultValue(true);
            }

            ConfigureAuditableEntity<User>(modelBuilder);
            ConfigureAuditableEntity<PendingAccount>(modelBuilder);
            ConfigureAuditableEntity<Role>(modelBuilder);
            ConfigureAuditableEntity<Permission>(modelBuilder);
            ConfigureAuditableEntity<Product>(modelBuilder);
            ConfigureAuditableEntity<Uom>(modelBuilder);
            ConfigureAuditableEntity<Warehouse>(modelBuilder);
            ConfigureAuditableEntity<MiscellaneousReceipt>(modelBuilder);
            ConfigureAuditableEntity<WarehouseReceiving>(modelBuilder);
            ConfigureAuditableEntity<MoveOrder>(modelBuilder);

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

            modelBuilder.Entity<MoveOrderProductWarehouseReceivings>()
                .HasKey(mowr => new { mowr.MoveOrderId, mowr.ProductId, mowr.WarehouseReceivingId });

            modelBuilder.Entity<MoveOrderProductWarehouseReceivings>()
                .HasOne(mowr => mowr.MoveOrderProduct)
                .WithMany(mop => mop.MoveOrderProductWarehouseReceivings)
                .HasForeignKey(mowr => new { mowr.MoveOrderId, mowr.ProductId })
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MoveOrderProductWarehouseReceivings>()
                .HasOne(mopwr => mopwr.WarehouseReceiving)
                .WithMany(wr => wr.MoveOrderProductWarehouseReceivings)
                .HasForeignKey(mopwr => mopwr.WarehouseReceivingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MiscellaneousReceipt>()
                .HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MiscellaneousReceipt>()
                .HasOne(d => d.Warehouse)
                .WithMany()
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

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

            modelBuilder.Entity<MoveOrder>()
                .HasOne(mo => mo.Warehouse)
                .WithMany()
                .HasForeignKey(mo => mo.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MoveOrderProducts>()
                .HasKey(mop => new { mop.MoveOrderId, mop.ProductId });

            modelBuilder.Entity<MoveOrderProducts>()
                .HasOne(mop => mop.MoveOrder)
                .WithMany(mo => mo.MoveOrderProducts)
                .HasForeignKey(mop => mop.MoveOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MoveOrderProducts>()
                .HasOne(mop => mop.Product)
                .WithMany()
                .HasForeignKey(mop => mop.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

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
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<UserRoles>().HasData(
                new UserRoles { UserId = 1, RoleId = 1 }
            );
        }

        private static void ConfigureAuditableEntity<T>(ModelBuilder modelBuilder) where T : class
        {
            modelBuilder.Entity<T>()
                .HasOne(nameof(BaseEntity.CreatedBy))
                .WithMany()
                .HasForeignKey(nameof(BaseEntity.CreatedById))
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<T>()
                .HasOne(nameof(BaseEntity.UpdatedBy))
                .WithMany()
                .HasForeignKey(nameof(BaseEntity.UpdatedById))
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}