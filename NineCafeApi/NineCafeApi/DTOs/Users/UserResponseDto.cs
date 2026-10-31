namespace NineCafeApi.DTOs.Users
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        // Role details flattened for easy display in table
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        // Timestamps for audit display
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}