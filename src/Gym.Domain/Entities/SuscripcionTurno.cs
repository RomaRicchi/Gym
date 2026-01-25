namespace Gym.Domain.Entities;

public class SuscripcionTurno : TenantEntity
{
    public int SuscripcionId { get; set; }
    public int TurnoPlantillaId { get; set; }
    public int? RutinaId { get; set; }

    // Navegación
    public virtual Suscripcion Suscripcion { get; set; } = null!;
    public virtual TurnoPlantilla TurnoPlantilla { get; set; } = null!;
    public virtual RutinaPlantilla? Rutina { get; set; }
}
