namespace Gym.Domain.Entities;

public class RutinaPlantillaEjercicio : TenantEntity
{
    public int RutinaId { get; set; }
    public int EjercicioId { get; set; }
    public int Orden { get; set; }
    public int Series { get; set; }
    public int Repeticiones { get; set; }
    public int DescansoSeg { get; set; }

    // Navegación
    public virtual RutinaPlantilla RutinaPlantilla { get; set; } = null!;
    public virtual Ejercicio Ejercicio { get; set; } = null!;
}
