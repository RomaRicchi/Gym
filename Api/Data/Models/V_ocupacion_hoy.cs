using System;
using System.Collections.Generic;

namespace Api.Data.Models;


public partial class V_ocupacion_hoy
{
    public uint Turno_id { get; set; }

    public DateOnly? Fecha { get; set; }

    public long Asistencias { get; set; }
}
