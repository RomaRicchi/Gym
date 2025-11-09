using System;
using Api.Data.Models;
public class Checkin
{
    public int Id { get; set; }
    public int SocioId { get; set; }
    public int TurnoPlantillaId { get; set; }
    public int? ProfesorId { get; set; }   // opcional
    public DateTime FechaHora { get; set; }
    public string? Observaciones { get; set; }

    // Relaciones
    public Socio Socio { get; set; } = null!;
    public TurnoPlantilla TurnoPlantilla { get; set; } = null!;
    public Personal? Profesor { get; set; }   // opcional
}
