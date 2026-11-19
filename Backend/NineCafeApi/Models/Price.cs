using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.Models
{
    public class Price
    {
        public int Id { get; set; }
        [Required]
        [Range(100, int.MaxValue, ErrorMessage = "Price must be positive and greater than zero.")]
        public int PriceRiel { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for related DailySale records
        public ICollection<DailySale> DailySales { get; set; } = new List<DailySale>();
    }
}
