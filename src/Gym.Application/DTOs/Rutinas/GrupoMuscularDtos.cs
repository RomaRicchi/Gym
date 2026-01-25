namespace Gym.Application.DTOs.Rutinas;

public class GrupoMuscularDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class GrupoMuscularCreateRequest
{
    public string Nombre { get; set; } = string.Empty;
}
