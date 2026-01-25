namespace Gym.Domain.Entities;

public class Sala : TenantEntity
{
    public string Nombre { get; set; } = null!;
    public int Cupo { get; set; }
    public bool Activa { get; set; } = true;

    // Navegación
    public virtual ICollection<TurnoPlantilla> TurnosPlantilla { get; set; } = new List<TurnoPlantilla>();
}
