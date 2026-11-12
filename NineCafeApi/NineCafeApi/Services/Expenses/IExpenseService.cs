using NineCafeApi.DTOs.Expenses;

namespace NineCafeApi.Services.Expenses
{
    public interface IExpenseService
    {
        Task<IEnumerable<ExpenseResponseDto>> GetAllExpensesAsync();
        Task<ExpenseResponseDto?> GetExpenseByIdAsync(int id);
        Task<ExpenseResponseDto> CreateExpenseAsync(ExpenseCreateDto dto);
        Task<ExpenseResponseDto?> UpdateExpenseAsync(int id, ExpenseCreateDto dto);
        Task<bool> DeleteExpenseAsync(int id);
    }
}
