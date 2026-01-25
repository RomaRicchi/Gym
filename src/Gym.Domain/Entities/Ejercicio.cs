namespace Gym.Domain.Entities;

public class Ejercicio : TenantEntity
{
    public string Nombre { get; set; } = null!;
    public string? Tips { get; set; }
    public string? MediaUrl { get; set; }
    public int GrupoMuscularId { get; set; }

    // Navegación
    public virtual GrupoMuscular GrupoMuscular { get; set; } = null!;
    public virtual ICollection<RutinaPlantillaEjercicio> RutinaPlantillaEjercicios { get; set; } = new List<RutinaPlantillaEjercicio>();
}
