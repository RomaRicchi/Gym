namespace Gym.Domain.Entities;

public class Comprobante : TenantEntity
{
    public string? FileUrl { get; set; }
    public string? MimeType { get; set; }
    public DateTime SubidoEn { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual OrdenPago? OrdenPago { get; set; }
}
