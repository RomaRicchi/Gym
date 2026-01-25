namespace Gym.Domain.Entities;

public class Checkin : TenantEntity
{
    public int SocioId { get; set; }
    public int TurnoPlantillaId { get; set; }
    public int? ProfesorId { get; set; }
    public DateTime FechaHora { get; set; }
    public string? Observaciones { get; set; }

    // Navegación
    public virtual Socio Socio { get; set; } = null!;
    public virtual TurnoPlantilla TurnoPlantilla { get; set; } = null!;
    public virtual Personal? Profesor { get; set; }
}
