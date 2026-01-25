namespace Gym.Domain.Entities;

public class Personal : TenantEntity
{
    public string Nombre { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Especialidad { get; set; }
    public bool Estado { get; set; } = true;

    // Navegación
    public virtual Usuario? Usuario { get; set; }
    public virtual ICollection<TurnoPlantilla> TurnosPlantilla { get; set; } = new List<TurnoPlantilla>();
    public virtual ICollection<Checkin> Checkins { get; set; } = new List<Checkin>();
}
