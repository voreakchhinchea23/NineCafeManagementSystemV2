namespace NineCafeApi.DTOs.Dashboard
{
    public class DailySaleResponseDto
    {
        public int Id { get; set; }
        public int PriceId { get; set; }
        public int UnitPrice { get; set; }
        public int CurrentPriceRiel { get; set; }
        public int Quantity { get; set; }
        public int Total => Quantity * UnitPrice;
        public DateOnly SaleDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
