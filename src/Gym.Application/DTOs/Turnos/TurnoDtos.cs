namespace Gym.Application.DTOs.Turnos;

public class TurnoPlantillaDto
{
    public int Id { get; set; }
    public int SalaId { get; set; }
    public string? SalaNombre { get; set; }
    public int PersonalId { get; set; }
    public string? PersonalNombre { get; set; }
    public int DiaSemanaId { get; set; }
    public string? DiaSemana { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public int DuracionMin { get; set; }
    public bool Activo { get; set; }
    public int? CupoDisponible { get; set; }
}

public class TurnoPlantillaCreateRequest
{
    public int SalaId { get; set; }
    public int PersonalId { get; set; }
    public int DiaSemanaId { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public int DuracionMin { get; set; }
    public bool Activo { get; set; } = true;
}

public class TurnoPlantillaUpdateRequest
{
    public int SalaId { get; set; }
    public int PersonalId { get; set; }
    public int DiaSemanaId { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public int DuracionMin { get; set; }
    public bool Activo { get; set; }
}
