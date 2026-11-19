using NineCafeApi.DTOs.AuditLogs;

namespace NineCafeApi.Services.AuditLogs
{
    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLogResponseDto>> GetAllAuditLogsAsync(int limit = 100, string? entityType = null, string? action = null);
    }
}
