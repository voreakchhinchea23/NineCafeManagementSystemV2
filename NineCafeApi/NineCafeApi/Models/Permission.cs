using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.Models
{
    public class Permission
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key to Module
        public int ModuleId { get; set; }
        public Module? Module { get; set; }


        // Navigation property for RolePermission
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}

