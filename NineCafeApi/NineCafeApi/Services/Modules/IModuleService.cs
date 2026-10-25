using NineCafeApi.DTOs.Modules;

namespace NineCafeApi.Services.Modules
{
    public interface IModuleService
    {
        Task<IEnumerable<ModuleResponseDto>> GetAllAsync();
        Task<ModuleResponseDto?> GetByIdAsync(int id);
        Task<ModuleResponseDto> CreateAsync(CreateModuleDto dto);
        Task<ModuleResponseDto?> UpdateAsync(int id, CreateModuleDto dto);
        Task<bool> DeleteAsync(int id);
    }
}