using NineCafeApi.DTOs.Debts;

namespace NineCafeApi.Services.Debts
{
    public interface IDebtService
    {
        Task<IEnumerable<DebtResponseDto>> GetAllAsync();
        Task<DebtResponseDto?> GetByIdAsync(int id);
        Task<DebtResponseDto> CreateAsync(DebtCreateDto dto);
        Task<DebtResponseDto?> UpdateAsync(int id, DebtCreateDto dto);
        Task<DebtResponseDto?> MarkAsPaidAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
