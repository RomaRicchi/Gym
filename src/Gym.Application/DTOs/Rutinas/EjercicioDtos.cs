namespace Gym.Application.DTOs.Rutinas;

public class EjercicioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Tips { get; set; }
    public string? MediaUrl { get; set; }
    public int GrupoMuscularId { get; set; }
    public string? GrupoMuscularNombre { get; set; }
}

public class EjercicioCreateRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Tips { get; set; }
    public string? MediaUrl { get; set; }
    public int GrupoMuscularId { get; set; }
}

public class EjercicioUpdateRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Tips { get; set; }
    public string? MediaUrl { get; set; }
    public int GrupoMuscularId { get; set; }
}
