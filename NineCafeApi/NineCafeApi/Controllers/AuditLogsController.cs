using Microsoft.AspNetCore.Mvc;
using NineCafeApi.Authorization;
using NineCafeApi.DTOs.AuditLogs;
using NineCafeApi.Services.AuditLogs;

namespace NineCafeApi.Controllers
{
    [Route("api/audit-logs")]
    [ApiController]
    [HasPermission(Perm.AUDIT_READ)]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        // GET: api/audit-logs?limit=100&entityType=DailySale&action=update
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLogResponseDto>>> GetAuditLogs(
            [FromQuery] int limit = 100,
            [FromQuery] string? entityType = null,
            [FromQuery] string? action = null)
        {
            var logs = await _auditLogService.GetAllAuditLogsAsync(limit, entityType, action);
            return Ok(logs);
        }
    }
}
