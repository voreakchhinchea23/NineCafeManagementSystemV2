using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.DTOs.Prices
{
    public class CreatePriceDto
    {
        [Required]
        [Range(100, int.MaxValue, ErrorMessage = "Price in Riel must be at least 100.")]
        public int PriceRiel { get; set; }
    }
}
