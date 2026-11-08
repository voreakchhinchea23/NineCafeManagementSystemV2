using NineCafeApi.DTOs.Withdrawals;

namespace NineCafeApi.Services.Withdrawals
{
    public interface IWithdrawalService
    {
        Task<IEnumerable<WithdrawalResponseDto>> GetAllAsync(DateOnly? date = null);
        Task<WithdrawalResponseDto?> GetByIdAsync(int id);
        Task<WithdrawalResponseDto> CreateAsync(WithdrawalCreateDto dto);
        Task<WithdrawalResponseDto?> UpdateAsync(int id, WithdrawalCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
