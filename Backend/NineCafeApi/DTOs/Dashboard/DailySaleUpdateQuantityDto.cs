using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.DTOs.Dashboard
{
    public class DailySaleUpdateQuantityDto
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or greater.")]
        public int Quantity { get; set; }
    }
}
