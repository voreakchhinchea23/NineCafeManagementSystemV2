using NineCafeApi.DTOs.Withdrawals;

namespace NineCafeApi.Services.Withdrawals
{
    public interface IWithdrawalService
    {
        Task<IEnumerable<WithdrawalResponseDto>> GetAllWithdrawalsAsync(DateOnly? date = null);
        Task<WithdrawalResponseDto?> GetWithdrawalByIdAsync(int id);
        Task<WithdrawalResponseDto> CreateWithdrawalAsync(WithdrawalCreateDto dto);
        Task<WithdrawalResponseDto?> UpdateWithdrawalAsync(int id, WithdrawalCreateDto dto);
        Task<bool> DeleteWithdrawalAsync(int id);
    }
}
