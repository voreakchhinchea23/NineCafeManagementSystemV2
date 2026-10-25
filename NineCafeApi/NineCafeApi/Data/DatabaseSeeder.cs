using Microsoft.EntityFrameworkCore;
using NineCafeApi.Authorization;
using NineCafeApi.Models;

namespace NineCafeApi.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // =========================================================
            // 1. SEED MODULES 
            // =========================================================
            var moduleNames = Perm.GetAll()
                .Select(p => p.Split('.')[0].ToUpper())
                .Distinct()
                .ToList();

            foreach (var moduleName in moduleNames)
            {
                var existingModule = await context.Modules
                    .FirstOrDefaultAsync(m => m.Name.ToUpper() == moduleName);

                if (existingModule == null)
                {
                    context.Modules.Add(new Module
                    {
                        Name = moduleName,
                        Description = $"Module for {moduleName.ToLower()} operations.",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }
            await context.SaveChangesAsync();

            // Cache modules into memory for quick ID lookup
            var modulesDict = await context.Modules
                .ToDictionaryAsync(m => m.Name.ToUpper(), m => m.Id);


            // =========================================================
            // 2. SEED ROLE ("Admin")
            // =========================================================
            var adminRole = await context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Name == "Admin");

            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Name = "Admin",
                    Description = "Administrator role with full permissions.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                context.Roles.Add(adminRole);
                await context.SaveChangesAsync();
            }


            // =========================================================
            // 3. SEED PERMISSIONS
            // =========================================================
            var allPermCodes = Perm.GetAll();

            foreach (var permCode in allPermCodes)
            {
                var existingPerm = await context.Permissions
                    .FirstOrDefaultAsync(p => p.Name.ToLower() == permCode.ToLower());

                if (existingPerm == null)
                {
                    var prefix = permCode.Split('.')[0].ToUpper();
                    int moduleId = modulesDict[prefix];

                    existingPerm = new Permission
                    {
                        Name = permCode,
                        Description = $"Allows {permCode}",
                        ModuleId = moduleId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    context.Permissions.Add(existingPerm);
                    await context.SaveChangesAsync();
                }

                // =========================================================
                // 4. ASSIGN PERMISSION TO "Admin" ROLE
                // =========================================================
                bool alreadyAssigned = adminRole.RolePermissions
                    .Any(rp => rp.PermissionId == existingPerm.Id);

                if (!alreadyAssigned)
                {
                    adminRole.RolePermissions.Add(new RolePermission
                    {
                        RoleId = adminRole.Id,
                        PermissionId = existingPerm.Id
                    });
                }
            }
            await context.SaveChangesAsync();


            // =========================================================
            // 5. SEED ADMIN USER
            // Username: admin
            // FullName: System Admin
            // Email: admin@ninecafe.local
            // Password: Abcd1234% (Hashed with BCrypt)
            // Phone: 012441593
            // IsActive: true
            // =========================================================
            bool adminUserExists = await context.Users
                .AnyAsync(u => u.Username.ToLower() == "admin" || u.Email.ToLower() == "admin@ninecafe.local");

            if (!adminUserExists)
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword("Abcd1234%");

                var adminUser = new User
                {
                    Username = "admin",
                    FullName = "System Admin",
                    Email = "admin@ninecafe.local",
                    PasswordHash = passwordHash,
                    PhoneNumber = "012441593",
                    IsActive = true,
                    RoleId = adminRole.Id, // Link directly to Admin role
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}