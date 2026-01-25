namespace Gym.Domain.Entities;

public class EvolucionFisica : TenantEntity
{
    public int SocioId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public decimal Peso { get; set; }
    public decimal Altura { get; set; }
    public decimal? Imc { get; set; }
    public decimal? PesoIdeal { get; set; }
    public decimal? Pecho { get; set; }
    public decimal? Cintura { get; set; }
    public decimal? Cadera { get; set; }
    public decimal? Brazo { get; set; }
    public decimal? Pierna { get; set; }
    public decimal? Gemelo { get; set; }
    public string? Observacion { get; set; }

    // Navegación
    public virtual Socio Socio { get; set; } = null!;
}
