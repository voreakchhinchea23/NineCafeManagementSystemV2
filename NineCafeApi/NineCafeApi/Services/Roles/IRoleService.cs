using NineCafeApi.DTOs.Roles;

namespace NineCafeApi.Services.Roles
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponseDto>> GetRolesAsync();
        Task<RoleResponseDto?> GetRoleByIdAsync(int id);
        Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto dto);
        Task<bool> UpdateRoleAsync(CreateRoleDto dto, int id);
        Task<bool> DeleteRoleAsync(int id);
        Task<bool> AssignPermissionToRoleAsync(AssignPermissionsToRoleDto dto);
        Task<bool> AddSinglePermissionAsync(int roleId, int permissionId);
        Task<bool> RemoveSinglePermissionAsync(int roleId, int permissionId);
    }
}
