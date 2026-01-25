namespace Gym.Application.DTOs.Suscripciones;

public class SuscripcionDto
{
    public int Id { get; set; }
    public int SocioId { get; set; }
    public string? SocioNombre { get; set; }
    public int PlanId { get; set; }
    public string? PlanNombre { get; set; }
    public DateTime Inicio { get; set; }
    public DateTime Fin { get; set; }
    public bool Estado { get; set; }
    public int? OrdenPagoId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SuscripcionCreateRequest
{
    public int SocioId { get; set; }
    public int PlanId { get; set; }
    public int? OrdenPagoId { get; set; }
    public DateTime Inicio { get; set; }
    public DateTime Fin { get; set; }
}

public class SuscripcionUpdateRequest
{
    public DateTime Inicio { get; set; }
    public DateTime Fin { get; set; }
    public bool Estado { get; set; }
}
