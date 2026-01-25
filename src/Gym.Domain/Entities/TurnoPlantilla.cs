namespace Gym.Domain.Entities;

public class TurnoPlantilla : TenantEntity
{
    public bool Activo { get; set; } = true;
    public int DuracionMin { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public int DiaSemanaId { get; set; }
    public int PersonalId { get; set; }
    public int SalaId { get; set; }

    // Navegación
    public virtual DiaSemana? DiaSemana { get; set; }
    public virtual Personal? Personal { get; set; }
    public virtual Sala? Sala { get; set; }
    public virtual ICollection<SuscripcionTurno> SuscripcionTurnos { get; set; } = new List<SuscripcionTurno>();
    public virtual ICollection<Checkin> Checkins { get; set; } = new List<Checkin>();
}
