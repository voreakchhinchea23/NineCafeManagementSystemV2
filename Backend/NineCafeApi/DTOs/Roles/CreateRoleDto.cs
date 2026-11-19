using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.DTOs.Roles
{
    public class CreateRoleDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Description { get; set; }

    }
}