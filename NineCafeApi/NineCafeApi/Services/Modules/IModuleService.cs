using NineCafeApi.DTOs.Modules;

namespace NineCafeApi.Services.Modules
{
    public interface IModuleService
    {
        Task<IEnumerable<ModuleResponseDto>> GetAllModulesAsync();
        Task<ModuleResponseDto?> GetModuleByIdAsync(int id);
        Task<ModuleResponseDto> CreateModuleAsync(CreateModuleDto dto);
        Task<ModuleResponseDto?> UpdateModuleAsync(int id, CreateModuleDto dto);
        Task<bool> DeleteModuleAsync(int id);
    }
}
