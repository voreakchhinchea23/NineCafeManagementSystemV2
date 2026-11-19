using System.ComponentModel.DataAnnotations;

namespace NineCafeApi.DTOs.Modules
{
    public class CreateModuleDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(250)]
        public string? Description { get; set; }
    }
}
