namespace Gym.Domain.Entities;

public class OrdenPago : TenantEntity
{
    public int SocioId { get; set; }
    public int PlanId { get; set; }
    public int EstadoId { get; set; }
    public decimal Monto { get; set; }
    public DateTime? VenceEn { get; set; }
    public string? Notas { get; set; }
    public int? ComprobanteId { get; set; }

    // Navegación
    public virtual Socio Socio { get; set; } = null!;
    public virtual Plan Plan { get; set; } = null!;
    public virtual EstadoOrdenPago Estado { get; set; } = null!;
    public virtual Comprobante? Comprobante { get; set; }
    public virtual ICollection<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();
}
