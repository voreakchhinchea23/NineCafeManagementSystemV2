using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.DTOs.Suppliers
{
    public class SupplierCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ContactInfo { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Address { get; set; }

        [Required]
        [StringLength(255)]
        public string Product { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Note { get; set; }
    }
}
