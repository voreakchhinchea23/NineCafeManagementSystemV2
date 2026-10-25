using Microsoft.EntityFrameworkCore;
using NineCafeApi.Data;
using NineCafeApi.DTOs.Permissions;
using NineCafeApi.DTOs.Roles;
using NineCafeApi.Models;

namespace NineCafeApi.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;

        public RoleService(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL ROLES
        public async Task<IEnumerable<RoleResponseDto>> GetRolesAsync()
        {
            return await _context.Roles
                .Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .ToListAsync();
        }

        // GET ROLE BY ID (WITH FULL PERMISSIONS)
        public async Task<RoleResponseDto?> GetRoleByIdAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                        .ThenInclude(p => p!.Module)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null) return null;

            return new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt,
                Permissions = role.RolePermissions
                    .Where(rp => rp.Permission != null)
                    .Select(rp => new PermissionResponseDto
                    {
                        Id = rp.Permission!.Id,
                        Name = rp.Permission.Name,
                        Description = rp.Permission.Description,
                        ModuleId = rp.Permission.ModuleId,
                        ModuleName = rp.Permission.Module?.Name ?? string.Empty,
                        CreatedAt = rp.Permission.CreatedAt,
                        UpdatedAt = rp.Permission.UpdatedAt
                    })
                    .ToList()
            };
        }

        // CREATE ROLE
        public async Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto dto)
        {
            var trimmedName = dto.Name.Trim();

            var exists = await _context.Roles
                .AnyAsync(r => r.Name.ToLower() == trimmedName.ToLower());
            if (exists)
            {
                throw new InvalidOperationException($"Role '{dto.Name}' already exists.");
            }

            var role = new Role
            {
                Name = trimmedName,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt
            };
        }

        // UPDATE ROLE
        public async Task<bool> UpdateRoleAsync(CreateRoleDto dto, int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return false;

            var trimmedName = dto.Name.Trim();

            var exists = await _context.Roles
                .AnyAsync(r => r.Id != id && r.Name.ToLower() == trimmedName.ToLower());
            if (exists)
            {
                throw new InvalidOperationException($"Role '{dto.Name}' already exists.");
            }

            role.Name = trimmedName;
            role.Description = dto.Description;
            role.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE ROLE
        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return false;

            // Prevent deleting role if users are still assigned to it
            var hasAssignedUsers = await _context.Users.AnyAsync(u => u.RoleId == id);
            if (hasAssignedUsers)
            {
                throw new InvalidOperationException("Cannot delete this role because users are currently assigned to it.");
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        // ASSIGN BATCH OF PERMISSIONS TO ROLE (REPLACES EXISTING)
        public async Task<bool> AssignPermissionToRoleAsync(AssignPermissionsToRoleDto dto)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == dto.RoleId);

            if (role == null) return false;

            // Only grab valid permission IDs from database
            var validPermissionIds = await _context.Permissions
                .Where(p => dto.PermissionIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            // Clear previous mappings and add new ones
            role.RolePermissions.Clear();

            foreach (var permId in validPermissionIds)
            {
                role.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permId
                });
            }

            role.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // ADD SINGLE PERMISSION TO ROLE
        public async Task<bool> AddSinglePermissionAsync(int roleId, int permissionId)
        {
            var alreadyExists = await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (alreadyExists) return true;

            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId);
            var permissionExists = await _context.Permissions.AnyAsync(p => p.Id == permissionId);

            if (!roleExists || !permissionExists) return false;

            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            });

            await _context.SaveChangesAsync();
            return true;
        }

        // REMOVE SINGLE PERMISSION FROM ROLE
        public async Task<bool> RemoveSinglePermissionAsync(int roleId, int permissionId)
        {
            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (rolePermission == null) return false;

            _context.RolePermissions.Remove(rolePermission);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}