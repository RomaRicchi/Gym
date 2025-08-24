using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class v_ocupacion_hoy
{
    public uint turno_id { get; set; }

    public DateOnly? fecha { get; set; }

    public long asistencias { get; set; }
}
