namespace Gym.Infrastructure.Tenant;

/// <summary>
/// Service to get current tenant context
/// </summary>
public interface ITenantService
{
    int? GetCurrentTenantId();
    void SetCurrentTenant(int tenantId);
}
