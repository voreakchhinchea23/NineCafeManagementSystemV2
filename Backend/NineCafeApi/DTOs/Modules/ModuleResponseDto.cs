using NineCafeApi.DTOs.Permissions;

namespace NineCafeApi.DTOs.Modules
{
    public class ModuleResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Timestamps for audit display
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
