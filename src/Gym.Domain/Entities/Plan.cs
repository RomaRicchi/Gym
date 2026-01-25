namespace Gym.Domain.Entities;

public class Plan : TenantEntity
{
    public string Nombre { get; set; } = string.Empty;
    public int DiasPorSemana { get; set; }
    public decimal Precio { get; set; }
    public bool Activo { get; set; } = true;

    // Navegación
    public virtual ICollection<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();
    public virtual ICollection<OrdenPago> OrdenesPago { get; set; } = new List<OrdenPago>();
}
