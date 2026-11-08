using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace NineCafeApi.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private static readonly HashSet<string> SelfProfileAllowedPermissions = new(StringComparer.OrdinalIgnoreCase)
        {
            Perm.USER_READ,
            Perm.USER_UPDATE,
            Perm.USER_CHANGE_PASSWORD
        };

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // 1. If user is not authenticated, fail
            if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
            {
                return Task.CompletedTask;
            }

            // 2. Check if user's JWT contains the required permission claim
            var hasPermission = context.User.Claims
                .Any(c => c.Type == "permission" &&
                          string.Equals(c.Value, requirement.Permission, StringComparison.OrdinalIgnoreCase));

            if (hasPermission)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // 3. Allow authenticated users to view/update/change-password on their own profile (/api/users/{id})
            if (SelfProfileAllowedPermissions.Contains(requirement.Permission) &&
                context.Resource is HttpContext httpContext &&
                httpContext.Request.RouteValues.TryGetValue("id", out var routeIdValue) &&
                Guid.TryParse(routeIdValue?.ToString(), out Guid targetUserId))
            {
                var currentUserIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(currentUserIdClaim, out Guid currentUserId) && currentUserId == targetUserId)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
