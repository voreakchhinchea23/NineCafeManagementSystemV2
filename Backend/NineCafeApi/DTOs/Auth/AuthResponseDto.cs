namespace NineCafeApi.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }

        // Basic user info for the UI navbar / session
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        // Permissions list for client-side route guards and button visibility
        public List<string> Permissions { get; set; } = new();
    }
}