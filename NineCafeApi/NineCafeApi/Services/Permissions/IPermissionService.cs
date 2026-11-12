using NineCafeApi.DTOs.Permissions;

namespace NineCafeApi.Services.Permissions
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionResponseDto>> GetAllPermissionsAsync();
        Task<PermissionResponseDto?> GetPermissionByIdAsync(int id);
        Task<IEnumerable<PermissionResponseDto>> GetPermissionsByModuleIdAsync(int moduleId);
        Task<PermissionResponseDto> CreatePermissionAsync(CreatePermissionDto dto);
        Task<PermissionResponseDto?> UpdatePermissionAsync(int id, CreatePermissionDto dto);
        Task<bool> DeletePermissionAsync(int id);
    }
}
