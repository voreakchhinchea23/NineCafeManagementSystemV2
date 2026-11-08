using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NineCafeApi.Models;

namespace NineCafeApi.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        private static readonly HashSet<Type> AuditedTypes = new()
        {
            typeof(DailySale),
            typeof(Withdrawal),
            typeof(Debt),
            typeof(Expense),
            typeof(Price),
            typeof(Supplier)
        };

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            IHttpContextAccessor? httpContextAccessor = null) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Module> Modules { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Withdrawal> Withdrawals { get; set; }
        public DbSet<Debt> Debts { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<DailySale> DailySales { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // 1. RolePermission (Role <-> Permission)
            // ==========================================
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => rp.Id);

            modelBuilder.Entity<RolePermission>()
                .HasIndex(rp => new { rp.RoleId, rp.PermissionId })
                .IsUnique();

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==========================================
            // 2. UserRole (User <-> Role)
            // ==========================================
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => ur.Id);

            // A user can only be assigned to a specific role once
            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================
            // 3. Unique Constraints & Defaults
            // ==========================================
            modelBuilder.Entity<User>()
                .Property(u => u.PublicId)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PublicId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<Module>()
                .HasIndex(m => m.Name)
                .IsUnique();

            // ==========================================
            // 4. Expense (User -> Expenses)
            // ==========================================
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================
            // 5. DailySale (Price -> DailySales)
            // ==========================================
            modelBuilder.Entity<DailySale>()
                .HasOne(ds => ds.PriceRecord)
                .WithMany(p => p.DailySales)
                .HasForeignKey(ds => ds.PriceId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================
            // 6. AuditLog (User -> AuditLogs)
            // ==========================================
            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var pendingAudits = CapturePendingAuditEntries();

            var result = await base.SaveChangesAsync(cancellationToken);

            if (pendingAudits.Count > 0)
            {
                int? currentUserId = await ResolveCurrentUserIdAsync(cancellationToken);

                foreach (var pending in pendingAudits)
                {
                    int entityId = pending.EntityId ?? GetEntityId(pending.Entry.CurrentValues);
                    string? afterData = pending.Action == "delete"
                        ? null
                        : SerializeValues(pending.Entry.CurrentValues);

                    AuditLogs.Add(new AuditLog
                    {
                        UserId = currentUserId,
                        Action = pending.Action,
                        EntityType = pending.EntityType,
                        EntityId = entityId.ToString(),
                        BeforeData = pending.BeforeData,
                        AfterData = afterData,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await base.SaveChangesAsync(cancellationToken);
            }

            return result;
        }

        private List<PendingAuditEntry> CapturePendingAuditEntries()
        {
            ChangeTracker.DetectChanges();

            var list = new List<PendingAuditEntry>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (!AuditedTypes.Contains(entry.Entity.GetType()))
                    continue;

                if (entry.State != EntityState.Added &&
                    entry.State != EntityState.Modified &&
                    entry.State != EntityState.Deleted)
                    continue;

                string action = entry.State switch
                {
                    EntityState.Added => "create",
                    EntityState.Modified => "update",
                    EntityState.Deleted => "delete",
                    _ => "unknown"
                };

                string? beforeData = entry.State == EntityState.Added
                    ? null
                    : SerializeValues(entry.OriginalValues);

                int? entityId = entry.State == EntityState.Added
                    ? null
                    : GetEntityId(entry.OriginalValues);

                list.Add(new PendingAuditEntry
                {
                    Entry = entry,
                    Action = action,
                    EntityType = entry.Entity.GetType().Name,
                    EntityId = entityId,
                    BeforeData = beforeData
                });
            }

            return list;
        }

        private async Task<int?> ResolveCurrentUserIdAsync(CancellationToken cancellationToken)
        {
            var publicIdClaim = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(publicIdClaim) || !Guid.TryParse(publicIdClaim, out var publicId))
            {
                return null;
            }

            return await Users
                .AsNoTracking()
                .Where(u => u.PublicId == publicId)
                .Select(u => (int?)u.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        private static int GetEntityId(PropertyValues values)
        {
            var idProp = values.Properties.FirstOrDefault(p => p.Name == "Id");
            if (idProp != null && values[idProp] is int id)
            {
                return id;
            }
            return 0;
        }

        private static string SerializeValues(PropertyValues values)
        {
            var dict = new Dictionary<string, object?>();
            foreach (var prop in values.Properties)
            {
                dict[prop.Name] = values[prop];
            }
            return JsonSerializer.Serialize(dict);
        }

        private sealed class PendingAuditEntry
        {
            public required EntityEntry Entry { get; init; }
            public required string Action { get; init; }
            public required string EntityType { get; init; }
            public int? EntityId { get; init; }
            public string? BeforeData { get; init; }
        }
    }
}
