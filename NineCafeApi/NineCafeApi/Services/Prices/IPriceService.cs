using NineCafeApi.DTOs.Prices;

namespace NineCafeApi.Services.Prices
{
    public interface IPriceService
    {
        Task<IEnumerable<PriceResponseDto>> GetAllPricesAsync();
        Task<PriceResponseDto?> GetPriceByIdAsync(int id);
        Task<PriceResponseDto> CreatePriceAsync(CreatePriceDto dto);
        Task<PriceResponseDto?> UpdatePriceAsync(int id, CreatePriceDto dto);
        Task<bool> DeletePriceAsync(int id);
    }
}
