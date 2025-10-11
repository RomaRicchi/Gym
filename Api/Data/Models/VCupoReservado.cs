using System;

namespace Api.Data.Models;

public partial class VCupoReservado
{
    public uint TurnoId { get; set; }

    public sbyte DiaSemana { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public int Cupo { get; set; }

    public long Reservados { get; set; }
}
