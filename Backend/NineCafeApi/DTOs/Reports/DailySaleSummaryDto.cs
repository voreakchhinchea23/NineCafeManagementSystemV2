namespace NineCafeApi.DTOs.Reports
{
    public class DailySaleSummaryDto
    {
        public DateOnly Date { get; set; }
        public int Day { get; set; }
        public int CupsSold { get; set; }
        public decimal Amount { get; set; }
    }
}
