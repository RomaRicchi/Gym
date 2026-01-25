namespace Gym.Domain.Entities;

/// <summary>
/// Represents a gym/tenant in the multi-tenant system
/// </summary>
public class Tenant : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Slug { get; set; } // Para subdominios: mi-gym.tuapp.com
    public string? Logo { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public bool Activo { get; set; } = true;

    // Plan de suscripción del gimnasio al SaaS
    public string PlanSaas { get; set; } = "free"; // free, pro, enterprise
    public DateTime? PlanVenceEn { get; set; }
}
