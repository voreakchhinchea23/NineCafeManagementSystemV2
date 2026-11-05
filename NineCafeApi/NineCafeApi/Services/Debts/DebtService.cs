using Microsoft.EntityFrameworkCore;
using NineCafeApi.Data;
using NineCafeApi.DTOs.Debts;
using NineCafeApi.Models;

namespace NineCafeApi.Services.Debts
{
    public class DebtService : IDebtService
    {
        private readonly AppDbContext _context;

        public DebtService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DebtResponseDto>> GetAllAsync()
        {
            return await _context.Debts
                .Select(d => new DebtResponseDto
                {
                    Id = d.Id,
                    CustomerName = d.CustomerName,
                    Amount = d.Amount,
                    Date = d.Date,
                    IsPaid = d.IsPaid,
                    PaidAt = d.PaidAt,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<DebtResponseDto?> GetByIdAsync(int id)
        {
            var debt = await _context.Debts.FindAsync(id);
            if (debt == null) return null;

            return new DebtResponseDto
            {
                Id = debt.Id,
                CustomerName = debt.CustomerName,
                Amount = debt.Amount,
                Date = debt.Date,
                IsPaid = debt.IsPaid,
                PaidAt = debt.PaidAt,
                CreatedAt = debt.CreatedAt,
                UpdatedAt = debt.UpdatedAt
            };
        }

        public async Task<DebtResponseDto> CreateAsync(DebtCreateDto dto)
        {
            var now = DateTime.UtcNow;

            var debt = new Debt
            {
                CustomerName = dto.CustomerName,
                Amount = dto.Amount,
                Date = dto.Date,
                IsPaid = dto.IsPaid,
                PaidAt = dto.IsPaid ? now : null,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.Debts.Add(debt);
            await _context.SaveChangesAsync();

            return new DebtResponseDto
            {
                Id = debt.Id,
                CustomerName = debt.CustomerName,
                Amount = debt.Amount,
                Date = debt.Date,
                IsPaid = debt.IsPaid,
                PaidAt = debt.PaidAt,
                CreatedAt = debt.CreatedAt,
                UpdatedAt = debt.UpdatedAt
            };
        }

        public async Task<DebtResponseDto?> UpdateAsync(int id, DebtCreateDto dto)
        {
            var debt = await _context.Debts.FindAsync(id);
            if (debt == null) return null;

            var now = DateTime.UtcNow;

            if (!debt.IsPaid && dto.IsPaid)
            {
                debt.PaidAt = now;
            }
            else if (!dto.IsPaid)
            {
                debt.PaidAt = null;
            }

            debt.CustomerName = dto.CustomerName;
            debt.Amount = dto.Amount;
            debt.Date = dto.Date;
            debt.IsPaid = dto.IsPaid;
            debt.UpdatedAt = now;

            await _context.SaveChangesAsync();

            return new DebtResponseDto
            {
                Id = debt.Id,
                CustomerName = debt.CustomerName,
                Amount = debt.Amount,
                Date = debt.Date,
                IsPaid = debt.IsPaid,
                PaidAt = debt.PaidAt,
                CreatedAt = debt.CreatedAt,
                UpdatedAt = debt.UpdatedAt
            };
        }

        public async Task<DebtResponseDto?> MarkAsPaidAsync(int id)
        {
            var debt = await _context.Debts.FindAsync(id);
            if (debt == null) return null;

            var now = DateTime.UtcNow;
            if (!debt.IsPaid)
            {
                debt.IsPaid = true;
                debt.PaidAt = now;
                debt.UpdatedAt = now;
                await _context.SaveChangesAsync();
            }

            return new DebtResponseDto
            {
                Id = debt.Id,
                CustomerName = debt.CustomerName,
                Amount = debt.Amount,
                Date = debt.Date,
                IsPaid = debt.IsPaid,
                PaidAt = debt.PaidAt,
                CreatedAt = debt.CreatedAt,
                UpdatedAt = debt.UpdatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var debt = await _context.Debts.FindAsync(id);
            if (debt == null) return false;

            _context.Debts.Remove(debt);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
