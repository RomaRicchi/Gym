using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class turno_plantilla
{
    public uint id { get; set; }

    public uint sala_id { get; set; }

    public uint? profesor_id { get; set; }

    public sbyte dia_semana { get; set; }

    public TimeOnly hora_inicio { get; set; }

    public int duracion_min { get; set; }

    public int cupo { get; set; }

    public bool? activo { get; set; }

    public virtual ICollection<checkin> checkin { get; set; } = new List<checkin>();

    public virtual ICollection<orden_turno> orden_turno { get; set; } = new List<orden_turno>();

    public virtual profesor? profesor { get; set; }

    public virtual sala sala { get; set; } = null!;

    public virtual ICollection<suscripcion_turno> suscripcion_turno { get; set; } = new List<suscripcion_turno>();
}
