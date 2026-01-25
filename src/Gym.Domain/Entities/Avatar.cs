namespace Gym.Domain.Entities;

public class Avatar : BaseEntity
{
    public string Url { get; set; } = null!;
    public string? Nombre { get; set; }
    public bool EsPredeterminado { get; set; }

    // Navegación
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
