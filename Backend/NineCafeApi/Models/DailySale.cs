using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NineCafeApi.Models
{
    public class DailySale
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        // Historical snapshot of Price.PriceRiel at the time of sale
        [Required]
        [Range(100, int.MaxValue, ErrorMessage = "Unit price must be at least 100 Riel.")]
        public int UnitPrice { get; set; }

        public DateOnly SaleDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Required]
        public int PriceId { get; set; }
        public Price PriceRecord { get; set; } = null!;

        // Calculated Total = Quantity * UnitPrice (using the historical snapshot UnitPrice)
        [NotMapped]
        public int Total => Quantity * UnitPrice;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
