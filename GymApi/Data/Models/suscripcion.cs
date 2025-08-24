using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class suscripcion
{
    public uint id { get; set; }

    public uint socio_id { get; set; }

    public uint plan_id { get; set; }

    public DateTime inicio { get; set; }

    public DateTime fin { get; set; }

    public string estado { get; set; } = null!;

    public DateTime creado_en { get; set; }

    public virtual plan plan { get; set; } = null!;

    public virtual socio socio { get; set; } = null!;

    public virtual ICollection<suscripcion_turno> suscripcion_turno { get; set; } = new List<suscripcion_turno>();
}
