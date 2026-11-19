using System.Globalization;

namespace NineCafeApi.DTOs.Reports
{
    public class MonthlyReportResponseDto
    {
        public const decimal ExchangeRate = 4000m;

        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month);

        public int TotalCupsSold { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Profit => TotalIncome - TotalExpense;

        public decimal TotalIncomeUsd => Math.Round(TotalIncome / ExchangeRate, 2);
        public decimal TotalExpenseUsd => Math.Round(TotalExpense / ExchangeRate, 2);
        public decimal ProfitUsd => Math.Round(Profit / ExchangeRate, 2);

        public IEnumerable<DailySaleSummaryDto> DailySales { get; set; } = new List<DailySaleSummaryDto>();
        public IEnumerable<DailyExpenseSummaryDto> DailyExpenses { get; set; } = new List<DailyExpenseSummaryDto>();
    }
}
