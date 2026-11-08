namespace NineCafeApi.DTOs.AuditLogs
{
    public class AuditLogResponseDto
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string? BeforeData { get; set; }
        public string? AfterData { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UserPublicId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
