namespace Gym.Application.DTOs.Turnos;

public class CheckinDto
{
    public int Id { get; set; }
    public int SocioId { get; set; }
    public string? SocioNombre { get; set; }
    public int TurnoPlantillaId { get; set; }
    public string? DiaSemana { get; set; }
    public TimeSpan? HoraInicio { get; set; }
    public int? ProfesorId { get; set; }
    public string? ProfesorNombre { get; set; }
    public DateTime FechaHora { get; set; }
    public string? Observaciones { get; set; }
}

public class CheckinCreateRequest
{
    public int SocioId { get; set; }
    public int TurnoPlantillaId { get; set; }
    public int? ProfesorId { get; set; }
    public string? Observaciones { get; set; }
}
