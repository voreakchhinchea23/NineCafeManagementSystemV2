using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.DTOs.Dashboard
{
    public class DailySaleCreateDto
    {
        [Required]
        public int PriceId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        public DateOnly SaleDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }
}
