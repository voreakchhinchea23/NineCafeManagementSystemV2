using NineCafeApi.DTOs.Permissions;

namespace NineCafeApi.Services.Permissions
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionResponseDto>> GetAllAsync();
        Task<PermissionResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<PermissionResponseDto>> GetByModuleIdAsync(int moduleId);
        Task<PermissionResponseDto> CreateAsync(CreatePermissionDto dto);
        Task<PermissionResponseDto?> UpdateAsync(int id, CreatePermissionDto dto);
        Task<bool> DeleteAsync(int id);
    }
}