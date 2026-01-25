namespace Gym.Application.DTOs.Rutinas;

public class EvolucionFisicaDto
{
    public int Id { get; set; }
    public int SocioId { get; set; }
    public string? SocioNombre { get; set; }
    public DateTime Fecha { get; set; }
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
}

public class EvolucionFisicaCreateRequest
{
    public int SocioId { get; set; }
    public decimal Peso { get; set; }
    public decimal Altura { get; set; }
    public decimal? PesoIdeal { get; set; }
    public decimal? Pecho { get; set; }
    public decimal? Cintura { get; set; }
    public decimal? Cadera { get; set; }
    public decimal? Brazo { get; set; }
    public decimal? Pierna { get; set; }
    public decimal? Gemelo { get; set; }
    public string? Observacion { get; set; }
}
