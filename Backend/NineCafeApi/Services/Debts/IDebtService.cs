using NineCafeApi.DTOs.Debts;

namespace NineCafeApi.Services.Debts
{
    public interface IDebtService
    {
        Task<IEnumerable<DebtResponseDto>> GetAllDebtsAsync();
        Task<DebtResponseDto?> GetDebtByIdAsync(int id);
        Task<DebtResponseDto> CreateDebtAsync(DebtCreateDto dto);
        Task<DebtResponseDto?> UpdateDebtAsync(int id, DebtCreateDto dto);
        Task<DebtResponseDto?> MarkDebtAsPaidAsync(int id);
        Task<bool> DeleteDebtAsync(int id);
    }
}
