namespace Gym.Domain.Entities;

public class RutinaPlantilla : TenantEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Objetivo { get; set; }
    public int GrupoMuscularId { get; set; }
    public string? ImagenUrl { get; set; }

    // Navegación
    public virtual GrupoMuscular GrupoMuscular { get; set; } = null!;
    public virtual ICollection<RutinaPlantillaEjercicio> RutinaPlantillaEjercicios { get; set; } = new List<RutinaPlantillaEjercicio>();
    public virtual ICollection<SuscripcionTurno> SuscripcionTurnos { get; set; } = new List<SuscripcionTurno>();
}
