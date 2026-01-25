using System;

namespace Api.Data.Models;

public partial class VOcupacionHoy
{
    public int TurnoId { get; set; }

    public DateOnly? Fecha { get; set; }

    public long Asistencias { get; set; }
}
