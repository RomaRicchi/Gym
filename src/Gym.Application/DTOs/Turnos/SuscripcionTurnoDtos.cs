namespace Gym.Application.DTOs.Turnos;

public class SuscripcionTurnoDto
{
    public int Id { get; set; }
    public int SuscripcionId { get; set; }
    public string? SocioNombre { get; set; }
    public int TurnoPlantillaId { get; set; }
    public string? DiaSemana { get; set; }
    public TimeSpan? HoraInicio { get; set; }
    public int? RutinaId { get; set; }
    public string? RutinaNombre { get; set; }
}

public class SuscripcionTurnoCreateRequest
{
    public int SuscripcionId { get; set; }
    public int TurnoPlantillaId { get; set; }
    public int? RutinaId { get; set; }
}

public class SuscripcionTurnoUpdateRequest
{
    public int? RutinaId { get; set; }
}
