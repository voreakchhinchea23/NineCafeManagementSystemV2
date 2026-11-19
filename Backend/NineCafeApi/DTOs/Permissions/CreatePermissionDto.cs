using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.DTOs.Permissions
{
    public class CreatePermissionDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(250)]
        public string? Description { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A valid ModuleId is required.")]
        public int ModuleId { get; set; }
    }
}
