using Microsoft.EntityFrameworkCore;
using NineCafeApi.Data;
using NineCafeApi.DTOs.Dashboard;
using NineCafeApi.DTOs.Prices;
using NineCafeApi.DTOs.Withdrawals;
using NineCafeApi.Models;

namespace NineCafeApi.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardResponseDto> GetDashboardByDateAsync(DateOnly date)
        {
            var priceTiers = await _context.Prices
                .OrderBy(p => p.PriceRiel)
                .Select(p => new PriceResponseDto
                {
                    Id = p.Id,
                    PriceRiel = p.PriceRiel,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            var todaySales = await _context.DailySales
                .Include(s => s.PriceRecord)
                .Where(s => s.SaleDate == date)
                .OrderBy(s => s.UnitPrice)
                .Select(s => new DailySaleResponseDto
                {
                    Id = s.Id,
                    PriceId = s.PriceId,
                    UnitPrice = s.UnitPrice,
                    CurrentPriceRiel = s.PriceRecord.PriceRiel,
                    Quantity = s.Quantity,
                    SaleDate = s.SaleDate,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();

            var withdrawals = await _context.Withdrawals
                .Where(w => w.Date == date)
                .OrderByDescending(w => w.Id)
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

            var totalCupSold = todaySales.Sum(s => s.Quantity);
            var dailyIncome = todaySales.Sum(s => s.Quantity * s.UnitPrice);
            var totalWithdrawnToday = withdrawals.Sum(w => w.Amount);

            return new DashboardResponseDto
            {
                SelectedDate = date,
                PriceTiers = priceTiers,
                TodaySales = todaySales,
                TotalCupSold = totalCupSold,
                DailyIncome = dailyIncome,
                Withdrawals = withdrawals,
                TotalWithdrawnToday = totalWithdrawnToday
            };
        }

        public async Task<IEnumerable<DailySaleResponseDto>> GetAllSalesAsync(DateOnly? date = null)
        {
            var query = _context.DailySales
                .Include(s => s.PriceRecord)
                .AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(s => s.SaleDate == date.Value);
            }

            return await query
                .OrderByDescending(s => s.SaleDate)
                .ThenBy(s => s.UnitPrice)
                .Select(s => new DailySaleResponseDto
                {
                    Id = s.Id,
                    PriceId = s.PriceId,
                    UnitPrice = s.UnitPrice,
                    CurrentPriceRiel = s.PriceRecord.PriceRiel,
                    Quantity = s.Quantity,
                    SaleDate = s.SaleDate,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<DailySaleResponseDto?> GetSaleByIdAsync(int id)
        {
            var sale = await _context.DailySales
                .Include(s => s.PriceRecord)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return null;

            return new DailySaleResponseDto
            {
                Id = sale.Id,
                PriceId = sale.PriceId,
                UnitPrice = sale.UnitPrice,
                CurrentPriceRiel = sale.PriceRecord.PriceRiel,
                Quantity = sale.Quantity,
                SaleDate = sale.SaleDate,
                CreatedAt = sale.CreatedAt,
                UpdatedAt = sale.UpdatedAt
            };
        }

        public async Task<DailySaleResponseDto> RecordSaleAsync(DailySaleCreateDto dto)
        {
            // Fetch authoritative Price record from database and freeze snapshot UnitPrice
            var price = await _context.Prices.FindAsync(dto.PriceId);
            if (price == null)
            {
                throw new KeyNotFoundException($"Price with ID {dto.PriceId} does not exist.");
            }

            var now = DateTime.UtcNow;

            // Check if a sale record already exists for this price tier & snapshot unit price on the same date
            var existing = await _context.DailySales.FirstOrDefaultAsync(
                s => s.PriceId == price.Id &&
                     s.UnitPrice == price.PriceRiel &&
                     s.SaleDate == dto.SaleDate);

            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
                existing.UpdatedAt = now;
                await _context.SaveChangesAsync();

                return new DailySaleResponseDto
                {
                    Id = existing.Id,
                    PriceId = existing.PriceId,
                    UnitPrice = existing.UnitPrice,
                    CurrentPriceRiel = price.PriceRiel,
                    Quantity = existing.Quantity,
                    SaleDate = existing.SaleDate,
                    CreatedAt = existing.CreatedAt,
                    UpdatedAt = existing.UpdatedAt
                };
            }

            var sale = new DailySale
            {
                PriceId = price.Id,
                UnitPrice = price.PriceRiel, // Immutable historical snapshot
                Quantity = dto.Quantity,
                SaleDate = dto.SaleDate,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.DailySales.Add(sale);
            await _context.SaveChangesAsync();

            return new DailySaleResponseDto
            {
                Id = sale.Id,
                PriceId = sale.PriceId,
                UnitPrice = sale.UnitPrice,
                CurrentPriceRiel = price.PriceRiel,
                Quantity = sale.Quantity,
                SaleDate = sale.SaleDate,
                CreatedAt = sale.CreatedAt,
                UpdatedAt = sale.UpdatedAt
            };
        }

        public async Task<DailySaleResponseDto?> UpdateSaleQuantityAsync(int id, int quantity)
        {
            var sale = await _context.DailySales
                .Include(s => s.PriceRecord)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return null;

            if (quantity <= 0)
            {
                _context.DailySales.Remove(sale);
                await _context.SaveChangesAsync();
                return null;
            }

            sale.Quantity = quantity;
            sale.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new DailySaleResponseDto
            {
                Id = sale.Id,
                PriceId = sale.PriceId,
                UnitPrice = sale.UnitPrice,
                CurrentPriceRiel = sale.PriceRecord.PriceRiel,
                Quantity = sale.Quantity,
                SaleDate = sale.SaleDate,
                CreatedAt = sale.CreatedAt,
                UpdatedAt = sale.UpdatedAt
            };
        }

        public async Task<DailySaleResponseDto?> SetSaleQuantityByPriceAsync(int priceId, int quantity, DateOnly? date = null)
        {
            var targetDate = date ?? DateOnly.FromDateTime(DateTime.Today);

            var price = await _context.Prices.FindAsync(priceId);
            if (price == null)
            {
                throw new KeyNotFoundException($"Price with ID {priceId} does not exist.");
            }

            var existing = await _context.DailySales.FirstOrDefaultAsync(
                s => s.PriceId == priceId && s.SaleDate == targetDate);

            var now = DateTime.UtcNow;

            if (existing != null)
            {
                if (quantity > 0)
                {
                    existing.Quantity = quantity;
                    existing.UpdatedAt = now;
                    await _context.SaveChangesAsync();

                    return new DailySaleResponseDto
                    {
                        Id = existing.Id,
                        PriceId = existing.PriceId,
                        UnitPrice = existing.UnitPrice,
                        CurrentPriceRiel = price.PriceRiel,
                        Quantity = existing.Quantity,
                        SaleDate = existing.SaleDate,
                        CreatedAt = existing.CreatedAt,
                        UpdatedAt = existing.UpdatedAt
                    };
                }

                _context.DailySales.Remove(existing);
                await _context.SaveChangesAsync();
                return null;
            }

            if (quantity > 0)
            {
                var sale = new DailySale
                {
                    PriceId = price.Id,
                    UnitPrice = price.PriceRiel,
                    Quantity = quantity,
                    SaleDate = targetDate,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                _context.DailySales.Add(sale);
                await _context.SaveChangesAsync();

                return new DailySaleResponseDto
                {
                    Id = sale.Id,
                    PriceId = sale.PriceId,
                    UnitPrice = sale.UnitPrice,
                    CurrentPriceRiel = price.PriceRiel,
                    Quantity = sale.Quantity,
                    SaleDate = sale.SaleDate,
                    CreatedAt = sale.CreatedAt,
                    UpdatedAt = sale.UpdatedAt
                };
            }

            return null;
        }

        public async Task<bool> DeleteSaleAsync(int id)
        {
            var sale = await _context.DailySales.FindAsync(id);
            if (sale == null) return false;

            _context.DailySales.Remove(sale);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
