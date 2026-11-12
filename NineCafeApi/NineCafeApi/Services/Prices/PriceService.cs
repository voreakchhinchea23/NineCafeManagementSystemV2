using Microsoft.EntityFrameworkCore;
using NineCafeApi.Data;
using NineCafeApi.DTOs.Prices;
using NineCafeApi.Models;

namespace NineCafeApi.Services.Prices
{
    public class PriceService : IPriceService
    {
        private readonly AppDbContext _context;

        public PriceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PriceResponseDto>> GetAllPricesAsync()
        {
            return await _context.Prices
                .OrderBy(p => p.PriceRiel)
                .Select(p => new PriceResponseDto
                {
                    Id = p.Id,
                    PriceRiel = p.PriceRiel,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<PriceResponseDto?> GetPriceByIdAsync(int id)
        {
            var price = await _context.Prices.FindAsync(id);
            if (price == null) return null;

            return new PriceResponseDto
            {
                Id = price.Id,
                PriceRiel = price.PriceRiel,
                CreatedAt = price.CreatedAt,
                UpdatedAt = price.UpdatedAt
            };
        }

        public async Task<PriceResponseDto> CreatePriceAsync(CreatePriceDto dto)
        {
            // Prevent duplicate price tiers
            var exists = await _context.Prices.AnyAsync(p => p.PriceRiel == dto.PriceRiel);
            if (exists)
            {
                throw new InvalidOperationException($"Price of {dto.PriceRiel} Riel already exists.");
            }

            var price = new Price
            {
                PriceRiel = dto.PriceRiel,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Prices.Add(price);
            await _context.SaveChangesAsync();

            return new PriceResponseDto
            {
                Id = price.Id,
                PriceRiel = price.PriceRiel,
                CreatedAt = price.CreatedAt,
                UpdatedAt = price.UpdatedAt
            };
        }

        public async Task<PriceResponseDto?> UpdatePriceAsync(int id, CreatePriceDto dto)
        {
            var price = await _context.Prices.FindAsync(id);
            if (price == null) return null;

            // Check if another price already uses this value
            var exists = await _context.Prices.AnyAsync(p => p.Id != id && p.PriceRiel == dto.PriceRiel);
            if (exists)
            {
                throw new InvalidOperationException($"Price of {dto.PriceRiel} Riel already exists.");
            }

            price.PriceRiel = dto.PriceRiel;
            price.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new PriceResponseDto
            {
                Id = price.Id,
                PriceRiel = price.PriceRiel,
                CreatedAt = price.CreatedAt,
                UpdatedAt = price.UpdatedAt
            };
        }

        public async Task<bool> DeletePriceAsync(int id)
        {
            var price = await _context.Prices.FindAsync(id);
            if (price == null) return false;

            _context.Prices.Remove(price);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
