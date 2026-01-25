namespace Gym.Domain.Entities;

public class Suscripcion : TenantEntity
{
    public int SocioId { get; set; }
    public int PlanId { get; set; }
    public DateTime Inicio { get; set; }
    public DateTime Fin { get; set; }
    public bool Estado { get; set; } = true;
    public int? OrdenPagoId { get; set; }

    // Navegación
    public virtual Socio Socio { get; set; } = null!;
    public virtual Plan Plan { get; set; } = null!;
    public virtual OrdenPago? OrdenPago { get; set; }
    public virtual ICollection<SuscripcionTurno> SuscripcionTurnos { get; set; } = new List<SuscripcionTurno>();
}
