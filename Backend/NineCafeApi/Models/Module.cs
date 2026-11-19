using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.Models
{
    public class Module
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for related permissions
        public ICollection<Permission>? Permissions { get; set; } = new List<Permission>();
    }
}

