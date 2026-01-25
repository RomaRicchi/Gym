namespace Gym.Application.DTOs.Planes;

public class PlanDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int DiasPorSemana { get; set; }
    public decimal Precio { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PlanCreateRequest
{
    public string Nombre { get; set; } = string.Empty;
    public int DiasPorSemana { get; set; }
    public decimal Precio { get; set; }
    public bool Activo { get; set; } = true;
}

public class PlanUpdateRequest
{
    public string Nombre { get; set; } = string.Empty;
    public int DiasPorSemana { get; set; }
    public decimal Precio { get; set; }
    public bool Activo { get; set; }
}
