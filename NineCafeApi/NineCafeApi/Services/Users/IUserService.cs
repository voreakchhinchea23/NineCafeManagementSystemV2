using NineCafeApi.DTOs.Users;

namespace NineCafeApi.Services.Users
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetUsersAsync();
        Task<UserResponseDto?> GetUserByIdAsync(Guid id);
        Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);
        Task<bool> UpdateUserAsync(Guid id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(Guid id);
        Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto dto);
    }
}