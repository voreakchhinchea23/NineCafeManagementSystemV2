using Microsoft.EntityFrameworkCore;
using NineCafeApi.Data;
using NineCafeApi.DTOs.AuditLogs;

namespace NineCafeApi.Services.AuditLogs
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _context;

        public AuditLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuditLogResponseDto>> GetAllAuditLogsAsync(
            int limit = 100,
            string? entityType = null,
            string? action = null)
        {
            var clampedLimit = Math.Clamp(limit, 1, 200);

            var query = _context.AuditLogs
                .AsNoTracking()
                .Include(a => a.User)
                    .ThenInclude(u => u!.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(entityType))
            {
                var trimmedType = entityType.Trim().ToLower();
                query = query.Where(a => a.EntityType.ToLower() == trimmedType);
            }

            if (!string.IsNullOrWhiteSpace(action))
            {
                var trimmedAction = action.Trim().ToLower();
                query = query.Where(a => a.Action.ToLower() == trimmedAction);
            }

            var logs = await query
                .OrderByDescending(a => a.Id)
                .Take(clampedLimit)
                .ToListAsync();

            return logs.Select(a => new AuditLogResponseDto
            {
                Id = a.Id,
                Action = a.Action,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                BeforeData = a.BeforeData,
                AfterData = a.AfterData,
                CreatedAt = a.CreatedAt,
                UserPublicId = a.User?.PublicId,
                UserName = a.User != null ? a.User.FullName : "System / Deleted User",
                RoleName = a.User?.UserRoles.FirstOrDefault()?.Role?.Name ?? "N/A"
            });
        }
    }
}
