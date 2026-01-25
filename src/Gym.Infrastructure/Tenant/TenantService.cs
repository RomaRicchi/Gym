namespace Gym.Infrastructure.Tenant;

/// <summary>
/// Implementation of tenant service using AsyncLocal for thread-safe tenant context
/// </summary>
public class TenantService : ITenantService
{
    private static readonly AsyncLocal<int?> _currentTenantId = new();

    public int? GetCurrentTenantId() => _currentTenantId.Value;

    public void SetCurrentTenant(int tenantId)
    {
        _currentTenantId.Value = tenantId;
    }
}
