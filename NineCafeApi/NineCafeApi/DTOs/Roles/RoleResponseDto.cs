using NineCafeApi.DTOs.Permissions;

namespace NineCafeApi.DTOs.Roles
{
    public class RoleResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<PermissionResponseDto> Permissions { get; set; } = new();

        // Timestamps for audit display
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}