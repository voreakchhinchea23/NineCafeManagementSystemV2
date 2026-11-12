using Microsoft.EntityFrameworkCore;
using NineCafeApi.Data;
using NineCafeApi.DTOs.Expenses;
using NineCafeApi.Models;

namespace NineCafeApi.Services.Expenses
{
    public class ExpenseService : IExpenseService
    {
        private readonly AppDbContext _context;

        public ExpenseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExpenseResponseDto>> GetAllExpensesAsync()
        {
            return await _context.Expenses
                .Include(e => e.User)
                .Select(e => new ExpenseResponseDto
                {
                    Id = e.Id,
                    Date = e.Date,
                    Amount = e.Amount,
                    Description = e.Description,
                    Other = e.Other,
                    UserPublicId = e.User.PublicId,
                    CreatedBy = e.User.FullName,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<ExpenseResponseDto?> GetExpenseByIdAsync(int id)
        {
            var expense = await _context.Expenses
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (expense == null) return null;

            return new ExpenseResponseDto
            {
                Id = expense.Id,
                Date = expense.Date,
                Amount = expense.Amount,
                Description = expense.Description,
                Other = expense.Other,
                UserPublicId = expense.User.PublicId,
                CreatedBy = expense.User.FullName,
                CreatedAt = expense.CreatedAt,
                UpdatedAt = expense.UpdatedAt
            };
        }

        public async Task<ExpenseResponseDto> CreateExpenseAsync(ExpenseCreateDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PublicId == dto.UserPublicId);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with PublicId '{dto.UserPublicId}' does not exist.");
            }

            var now = DateTime.UtcNow;

            var expense = new Expense
            {
                Date = dto.Date,
                Amount = dto.Amount,
                Description = dto.Description,
                Other = dto.Other,
                UserId = user.Id,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return new ExpenseResponseDto
            {
                Id = expense.Id,
                Date = expense.Date,
                Amount = expense.Amount,
                Description = expense.Description,
                Other = expense.Other,
                UserPublicId = user.PublicId,
                CreatedBy = user.FullName,
                CreatedAt = expense.CreatedAt,
                UpdatedAt = expense.UpdatedAt
            };
        }

        public async Task<ExpenseResponseDto?> UpdateExpenseAsync(int id, ExpenseCreateDto dto)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return null;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.PublicId == dto.UserPublicId);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with PublicId '{dto.UserPublicId}' does not exist.");
            }

            expense.Date = dto.Date;
            expense.Amount = dto.Amount;
            expense.Description = dto.Description;
            expense.Other = dto.Other;
            expense.UserId = user.Id;
            expense.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ExpenseResponseDto
            {
                Id = expense.Id,
                Date = expense.Date,
                Amount = expense.Amount,
                Description = expense.Description,
                Other = expense.Other,
                UserPublicId = user.PublicId,
                CreatedBy = user.FullName,
                CreatedAt = expense.CreatedAt,
                UpdatedAt = expense.UpdatedAt
            };
        }

        public async Task<bool> DeleteExpenseAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return false;

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
