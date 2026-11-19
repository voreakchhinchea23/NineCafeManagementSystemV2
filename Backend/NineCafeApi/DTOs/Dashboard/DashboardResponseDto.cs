using NineCafeApi.DTOs.Prices;
using NineCafeApi.DTOs.Withdrawals;

namespace NineCafeApi.DTOs.Dashboard
{
    public class DashboardResponseDto
    {
        public DateOnly SelectedDate { get; set; }
        public IEnumerable<PriceResponseDto> PriceTiers { get; set; } = new List<PriceResponseDto>();
        public IEnumerable<DailySaleResponseDto> TodaySales { get; set; } = new List<DailySaleResponseDto>();
        public int TotalCupSold { get; set; }
        public int DailyIncome { get; set; }
        public IEnumerable<WithdrawalResponseDto> Withdrawals { get; set; } = new List<WithdrawalResponseDto>();
        public decimal TotalWithdrawnToday { get; set; }
    }
}
