namespace Gym.Domain.Entities;

public class DiaSemana
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    // Navegación
    public virtual ICollection<TurnoPlantilla> TurnosPlantilla { get; set; } = new List<TurnoPlantilla>();
}
