using System;

namespace Api.Data.Models;

public partial class VOcupacionHoy
{
    public uint TurnoId { get; set; }

    public DateOnly? Fecha { get; set; }

    public long Asistencias { get; set; }
}
