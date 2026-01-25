using Gym.Infrastructure.Tenant;

namespace Gym.API.Middlewares;

/// <summary>
/// Middleware to extract tenant from JWT claims or header
/// </summary>
public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        // Try to get tenant from JWT claims
        var tenantClaim = context.User?.FindFirst("tenant_id")?.Value;

        if (!string.IsNullOrEmpty(tenantClaim) && int.TryParse(tenantClaim, out var tenantId))
        {
            tenantService.SetCurrentTenant(tenantId);
        }
        // Alternative: Get from header (for API calls)
        else if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var headerTenantId))
        {
            if (int.TryParse(headerTenantId, out var headerTenant))
            {
                tenantService.SetCurrentTenant(headerTenant);
            }
        }

        await _next(context);
    }
}

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantMiddleware>();
    }
}
