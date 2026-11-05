using NineCafeApi.DTOs.Expenses;

namespace NineCafeApi.Services.Expenses
{
    public interface IExpenseService
    {
        Task<IEnumerable<ExpenseResponseDto>> GetAllAsync();
        Task<ExpenseResponseDto?> GetByIdAsync(int id);
        Task<ExpenseResponseDto> CreateAsync(ExpenseCreateDto dto);
        Task<ExpenseResponseDto?> UpdateAsync(int id, ExpenseCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
