namespace Gym.Application.DTOs.Rutinas;

public class RutinaPlantillaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Objetivo { get; set; }
    public int GrupoMuscularId { get; set; }
    public string? GrupoMuscularNombre { get; set; }
    public string? ImagenUrl { get; set; }
}

public class RutinaPlantillaCreateRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Objetivo { get; set; }
    public int GrupoMuscularId { get; set; }
    public string? ImagenUrl { get; set; }
}

public class RutinaPlantillaUpdateRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Objetivo { get; set; }
    public int GrupoMuscularId { get; set; }
    public string? ImagenUrl { get; set; }
}

public class RutinaPlantillaDetalleDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Objetivo { get; set; }
    public int GrupoMuscularId { get; set; }
    public string? GrupoMuscularNombre { get; set; }
    public string? ImagenUrl { get; set; }
    public List<RutinaPlantillaEjercicioDto> Ejercicios { get; set; } = new();
}
