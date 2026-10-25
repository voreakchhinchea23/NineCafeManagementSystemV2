using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for RolePermission
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        // Navigation property for Users
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}

