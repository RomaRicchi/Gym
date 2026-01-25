namespace Gym.Application.DTOs.Rutinas;

public class RutinaPlantillaEjercicioDto
{
    public int Id { get; set; }
    public int RutinaId { get; set; }
    public string? RutinaNombre { get; set; }
    public int EjercicioId { get; set; }
    public string? EjercicioNombre { get; set; }
    public int Orden { get; set; }
    public int Series { get; set; }
    public int Repeticiones { get; set; }
    public int DescansoSeg { get; set; }
}

public class RutinaPlantillaEjercicioCreateRequest
{
    public int RutinaId { get; set; }
    public int EjercicioId { get; set; }
    public int Orden { get; set; }
    public int Series { get; set; }
    public int Repeticiones { get; set; }
    public int DescansoSeg { get; set; }
}

public class RutinaPlantillaEjercicioUpdateRequest
{
    public int Orden { get; set; }
    public int Series { get; set; }
    public int Repeticiones { get; set; }
    public int DescansoSeg { get; set; }
}
