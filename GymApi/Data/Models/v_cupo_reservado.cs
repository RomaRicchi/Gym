using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class v_cupo_reservado
{
    public uint turno_id { get; set; }

    public sbyte dia_semana { get; set; }

    public TimeOnly hora_inicio { get; set; }

    public int cupo { get; set; }

    public long reservados { get; set; }
}
