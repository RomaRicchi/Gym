using System;

namespace Api.Data.Models;

public partial class VCupoReservado
{
    public int TurnoId { get; set; }

    public byte DiaSemana { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public int Cupo { get; set; }

    public long Reservados { get; set; }
}
