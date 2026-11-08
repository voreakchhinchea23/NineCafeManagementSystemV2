using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.Models
{
    public class Supplier
    {
        public int Id { get; set; }

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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
