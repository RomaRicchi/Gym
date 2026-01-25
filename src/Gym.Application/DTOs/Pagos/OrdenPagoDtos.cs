namespace Gym.Application.DTOs.Pagos;

public class OrdenPagoDto
{
    public int Id { get; set; }
    public int SocioId { get; set; }
    public string? SocioNombre { get; set; }
    public int PlanId { get; set; }
    public string? PlanNombre { get; set; }
    public int EstadoId { get; set; }
    public string? EstadoNombre { get; set; }
    public decimal Monto { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? VenceEn { get; set; }
    public string? Notas { get; set; }
    public int? ComprobanteId { get; set; }
    public string? ComprobanteUrl { get; set; }
}

public class OrdenPagoCreateRequest
{
    public int SocioId { get; set; }
    public int PlanId { get; set; }
    public int EstadoId { get; set; }
    public DateTime FechaInicio { get; set; }
    public string? Notas { get; set; }
}

public class OrdenPagoUpdateEstadoRequest
{
    public int EstadoId { get; set; }
    public string? Notas { get; set; }
}

public class ComprobanteLinkRequest
{
    public int? ComprobanteId { get; set; }
}
