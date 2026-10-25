namespace NineCafeApi.DTOs.Permissions
{
    public class PermissionResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;

        // Timestamps for audit display
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
