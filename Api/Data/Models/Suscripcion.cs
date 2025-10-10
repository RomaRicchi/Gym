using System;
using System.Collections.Generic;

namespace Api.Data.Models;


public partial class Suscripcion
{
    public uint Id { get; set; }

    public uint Socio_id { get; set; }

    public uint Plan_id { get; set; }

    public DateTime Inicio { get; set; }

    public DateTime Fin { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime Creado_en { get; set; }

    public virtual plan Plan { get; set; } = null!;

    public virtual socio Socio { get; set; } = null!;

    public virtual ICollection<suscripcion_turno> Suscripcion_turno { get; set; } = new List<suscripcion_turno>();
}
