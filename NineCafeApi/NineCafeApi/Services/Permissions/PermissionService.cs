using Microsoft.EntityFrameworkCore;
using NineCafeApi.Data;
using NineCafeApi.DTOs.Permissions;
using NineCafeApi.Models;

namespace NineCafeApi.Services.Permissions
{
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _context;

        public PermissionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PermissionResponseDto>> GetAllAsync()
        {
            return await _context.Permissions
                .Include(p => p.Module)
                .Select(p => new PermissionResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ModuleId = p.ModuleId,
                    ModuleName = p.Module != null ? p.Module.Name : string.Empty,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<PermissionResponseDto?> GetByIdAsync(int id)
        {
            var permission = await _context.Permissions
                .Include(p => p.Module)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (permission == null) return null;

            return new PermissionResponseDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                ModuleId = permission.ModuleId,
                ModuleName = permission.Module?.Name ?? string.Empty,
                CreatedAt = permission.CreatedAt,
                UpdatedAt = permission.UpdatedAt
            };
        }

        public async Task<IEnumerable<PermissionResponseDto>> GetByModuleIdAsync(int moduleId)
        {
            return await _context.Permissions
                .Where(p => p.ModuleId == moduleId)
                .Include(p => p.Module)
                .Select(p => new PermissionResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ModuleId = p.ModuleId,
                    ModuleName = p.Module != null ? p.Module.Name : string.Empty,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<PermissionResponseDto> CreateAsync(CreatePermissionDto dto)
        {
            // 1. Verify module exists
            var module = await _context.Modules.FindAsync(dto.ModuleId);
            if (module == null)
            {
                throw new KeyNotFoundException($"Module with ID {dto.ModuleId} does not exist.");
            }

            var trimmedName = dto.Name.Trim();

            // 2. Check duplicate name
            var exists = await _context.Permissions
                .AnyAsync(p => p.Name.ToLower() == trimmedName.ToLower());
            if (exists)
            {
                throw new InvalidOperationException($"Permission with name '{dto.Name}' already exists.");
            }

            var permission = new Permission
            {
                Name = trimmedName,
                Description = dto.Description,
                ModuleId = dto.ModuleId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            return new PermissionResponseDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                ModuleId = permission.ModuleId,
                ModuleName = module.Name,
                CreatedAt = permission.CreatedAt,
                UpdatedAt = permission.UpdatedAt
            };
        }

        public async Task<PermissionResponseDto?> UpdateAsync(int id, CreatePermissionDto dto)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null) return null;

            // 1. Verify module exists
            var module = await _context.Modules.FindAsync(dto.ModuleId);
            if (module == null)
            {
                throw new KeyNotFoundException($"Module with ID {dto.ModuleId} does not exist.");
            }

            var trimmedName = dto.Name.Trim();

            // 2. Check duplicate name on other permissions
            var exists = await _context.Permissions
                .AnyAsync(p => p.Id != id && p.Name.ToLower() == trimmedName.ToLower());
            if (exists)
            {
                throw new InvalidOperationException($"Permission with name '{dto.Name}' already exists.");
            }

            permission.Name = trimmedName;
            permission.Description = dto.Description;
            permission.ModuleId = dto.ModuleId;
            permission.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new PermissionResponseDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                ModuleId = permission.ModuleId,
                ModuleName = module.Name,
                CreatedAt = permission.CreatedAt,
                UpdatedAt = permission.UpdatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null) return false;

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}