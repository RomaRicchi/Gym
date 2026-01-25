namespace Gym.Domain.Entities;

public class Rol : BaseEntity
{
    public string Nombre { get; set; } = null!;

    // Navegación
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
