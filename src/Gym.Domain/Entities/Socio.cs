namespace Gym.Domain.Entities;

public class Socio : TenantEntity
{
    public string Dni { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime? FechaNacimiento { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; } = true;

    // Navegación
    public virtual Usuario? Usuario { get; set; }
    public virtual ICollection<Checkin> Checkins { get; set; } = new List<Checkin>();
    public virtual ICollection<OrdenPago> OrdenesPago { get; set; } = new List<OrdenPago>();
    public virtual ICollection<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();
    public virtual ICollection<EvolucionFisica> EvolucionesFisicas { get; set; } = new List<EvolucionFisica>();
}
