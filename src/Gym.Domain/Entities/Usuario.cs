namespace Gym.Domain.Entities;

public class Usuario : TenantEntity
{
    public string Email { get; set; } = null!;
    public string? Alias { get; set; }
    public int RolId { get; set; }
    public int? PersonalId { get; set; }
    public int? SocioId { get; set; }
    public string PasswordHash { get; set; } = null!;
    public bool Estado { get; set; } = true;
    public int? AvatarId { get; set; }

    // Navegación
    public virtual Rol? Rol { get; set; }
    public virtual Personal? Personal { get; set; }
    public virtual Socio? Socio { get; set; }
    public virtual Avatar? Avatar { get; set; }
}
