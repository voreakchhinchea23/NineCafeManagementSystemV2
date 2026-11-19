namespace NineCafeApi.DTOs.Reports
{
    public class DailyExpenseSummaryDto
    {
        public DateOnly Date { get; set; }
        public int Day { get; set; }
        public decimal Amount { get; set; }
    }
}
