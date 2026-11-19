using NineCafeApi.DTOs.Auth;

namespace NineCafeApi.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    }
}
