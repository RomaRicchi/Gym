namespace Gym.Domain.Entities;

public class EstadoOrdenPago : BaseEntity
{
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    // Navegación
    public virtual ICollection<OrdenPago> OrdenesPago { get; set; } = new List<OrdenPago>();
}
