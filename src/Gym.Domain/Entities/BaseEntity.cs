namespace Gym.Domain.Entities;

/// <summary>
/// Base entity with common properties for all entities
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Base entity for multi-tenant entities
/// </summary>
public abstract class TenantEntity : BaseEntity
{
    public int TenantId { get; set; }
}
