using System;
using System.Collections.Generic;

namespace Api.Data.Models;


public partial class Turno_plantilla
{
    public uint Id { get; set; }

    public uint Sala_id { get; set; }

    public uint? Profesor_id { get; set; }

    public sbyte Dia_semana { get; set; }

    public TimeOnly Hora_inicio { get; set; }

    public int Duracion_min { get; set; }

    public int Cupo { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<checkin> Checkin { get; set; } = new List<checkin>();

    public virtual ICollection<orden_turno> Orden_turno { get; set; } = new List<orden_turno>();

    public virtual profesor? Profesor { get; set; }

    public virtual sala Sala { get; set; } = null!;

    public virtual ICollection<suscripcion_turno> Suscripcion_turno { get; set; } = new List<suscripcion_turno>();
}
