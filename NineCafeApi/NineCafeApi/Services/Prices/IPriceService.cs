using NineCafeApi.DTOs.Prices;

namespace NineCafeApi.Services.Prices
{
    public interface IPriceService
    {
        Task<IEnumerable<PriceResponseDto>> GetAllAsync();
        Task<PriceResponseDto?> GetByIdAsync(int id);
        Task<PriceResponseDto> CreateAsync(CreatePriceDto dto);
        Task<PriceResponseDto?> UpdateAsync(int id, CreatePriceDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
