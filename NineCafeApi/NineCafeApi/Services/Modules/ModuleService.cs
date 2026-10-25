using Microsoft.EntityFrameworkCore;
using NineCafeApi.Data;
using NineCafeApi.DTOs.Modules;
using NineCafeApi.Models;

namespace NineCafeApi.Services.Modules
{
    public class ModuleService : IModuleService
    {
        private readonly AppDbContext _context;

        public ModuleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ModuleResponseDto>> GetAllAsync()
        {
            return await _context.Modules
                .Select(m => new ModuleResponseDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<ModuleResponseDto?> GetByIdAsync(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null) return null;

            return new ModuleResponseDto
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description,
                CreatedAt = module.CreatedAt,
                UpdatedAt = module.UpdatedAt
            };
        }

        public async Task<ModuleResponseDto> CreateAsync(CreateModuleDto dto)
        {
            var module = new Module
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Modules.Add(module);
            await _context.SaveChangesAsync();

            return new ModuleResponseDto
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description,
                CreatedAt = module.CreatedAt,
                UpdatedAt = module.UpdatedAt
            };
        }

        public async Task<ModuleResponseDto?> UpdateAsync(int id, CreateModuleDto dto)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null) return null;

            module.Name = dto.Name;
            module.Description = dto.Description;
            module.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ModuleResponseDto
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description,
                CreatedAt = module.CreatedAt,
                UpdatedAt = module.UpdatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null) return false;

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}