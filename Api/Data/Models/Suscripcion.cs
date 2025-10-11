using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class Suscripcion
{
    public uint Id { get; set; }

    public uint SocioId { get; set; }

    public uint PlanId { get; set; }

    public DateTime Inicio { get; set; }

    public DateTime Fin { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime CreadoEn { get; set; }

    public virtual Plan Plan { get; set; } = null!;

    public virtual Socio Socio { get; set; } = null!;

    public virtual ICollection<SuscripcionTurno> SuscripcionesTurno { get; set; } = new List<SuscripcionTurno>();
}
