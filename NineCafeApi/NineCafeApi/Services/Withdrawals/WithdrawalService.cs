using Microsoft.EntityFrameworkCore;
using NineCafeApi.Data;
using NineCafeApi.DTOs.Withdrawals;
using NineCafeApi.Models;

namespace NineCafeApi.Services.Withdrawals
{
    public class WithdrawalService : IWithdrawalService
    {
        private readonly AppDbContext _context;

        public WithdrawalService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WithdrawalResponseDto>> GetAllWithdrawalsAsync(DateOnly? date = null)
        {
            var query = _context.Withdrawals.AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(w => w.Date == date.Value);
            }

            return await query
                .OrderByDescending(w => w.Date)
                .ThenByDescending(w => w.Id)
                .Select(w => new WithdrawalResponseDto
                {
                    Id = w.Id,
                    Amount = w.Amount,
                    Reason = w.Reason,
                    Date = w.Date,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<WithdrawalResponseDto?> GetWithdrawalByIdAsync(int id)
        {
            var withdrawal = await _context.Withdrawals.FindAsync(id);
            if (withdrawal == null) return null;

            return new WithdrawalResponseDto
            {
                Id = withdrawal.Id,
                Amount = withdrawal.Amount,
                Reason = withdrawal.Reason,
                Date = withdrawal.Date,
                CreatedAt = withdrawal.CreatedAt,
                UpdatedAt = withdrawal.UpdatedAt
            };
        }

        public async Task<WithdrawalResponseDto> CreateWithdrawalAsync(WithdrawalCreateDto dto)
        {
            var withdrawal = new Withdrawal
            {
                Amount = dto.Amount,
                Reason = dto.Reason,
                Date = dto.Date,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Withdrawals.Add(withdrawal);
            await _context.SaveChangesAsync();

            return new WithdrawalResponseDto
            {
                Id = withdrawal.Id,
                Amount = withdrawal.Amount,
                Reason = withdrawal.Reason,
                Date = withdrawal.Date,
                CreatedAt = withdrawal.CreatedAt,
                UpdatedAt = withdrawal.UpdatedAt
            };
        }

        public async Task<WithdrawalResponseDto?> UpdateWithdrawalAsync(int id, WithdrawalCreateDto dto)
        {
            var withdrawal = await _context.Withdrawals.FindAsync(id);
            if (withdrawal == null) return null;

            withdrawal.Amount = dto.Amount;
            withdrawal.Reason = dto.Reason;
            withdrawal.Date = dto.Date;
            withdrawal.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new WithdrawalResponseDto
            {
                Id = withdrawal.Id,
                Amount = withdrawal.Amount,
                Reason = withdrawal.Reason,
                Date = withdrawal.Date,
                CreatedAt = withdrawal.CreatedAt,
                UpdatedAt = withdrawal.UpdatedAt
            };
        }

        public async Task<bool> DeleteWithdrawalAsync(int id)
        {
            var withdrawal = await _context.Withdrawals.FindAsync(id);
            if (withdrawal == null) return false;

            _context.Withdrawals.Remove(withdrawal);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
