using System;
using System.Collections.Generic;

namespace Api.Data.Models;


public partial class V_cupo_reservado
{
    public uint Turno_id { get; set; }

    public sbyte Dia_semana { get; set; }

    public TimeOnly Hora_inicio { get; set; }

    public int Cupo { get; set; }

    public long Reservados { get; set; }
}
