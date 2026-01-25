namespace Gym.Application.DTOs.Turnos;

public class SalaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Cupo { get; set; }
    public bool Activa { get; set; }
    public int? CupoDisponible { get; set; }
}

public class SalaCreateRequest
{
    public string Nombre { get; set; } = string.Empty;
    public int Cupo { get; set; }
    public bool Activa { get; set; } = true;
}

public class SalaUpdateRequest
{
    public string Nombre { get; set; } = string.Empty;
    public int Cupo { get; set; }
    public bool Activa { get; set; }
}
