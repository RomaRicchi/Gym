namespace Gym.Domain.Entities;

public class GrupoMuscular : TenantEntity
{
    public string Nombre { get; set; } = string.Empty;

    // Navegación
    public virtual ICollection<Ejercicio> Ejercicios { get; set; } = new List<Ejercicio>();
    public virtual ICollection<RutinaPlantilla> RutinasPlantilla { get; set; } = new List<RutinaPlantilla>();
}
