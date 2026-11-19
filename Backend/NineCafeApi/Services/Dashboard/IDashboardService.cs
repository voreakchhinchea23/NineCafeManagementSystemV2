using NineCafeApi.DTOs.Dashboard;

namespace NineCafeApi.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardResponseDto> GetDashboardByDateAsync(DateOnly date);
        Task<IEnumerable<DailySaleResponseDto>> GetAllSalesAsync(DateOnly? date = null);
        Task<DailySaleResponseDto?> GetSaleByIdAsync(int id);
        Task<DailySaleResponseDto> RecordSaleAsync(DailySaleCreateDto dto);
        Task<DailySaleResponseDto?> UpdateSaleQuantityAsync(int id, int quantity);
        Task<DailySaleResponseDto?> SetSaleQuantityByPriceAsync(int priceId, int quantity, DateOnly? date = null);
        Task<bool> DeleteSaleAsync(int id);
    }
}
