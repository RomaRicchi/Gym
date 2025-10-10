using System;
using System.Collections.Generic;
namespace Api.Data.Models;


public partial class Suscripcion_turno
{    public uint Id { get; set; }

    public uint Suscripcion_id { get; set; }

    public uint Turno_plantilla_id { get; set; }

    public virtual suscripcion Suscripcion { get; set; } = null!;

    public virtual turno_plantilla Turno_plantilla { get; set; } = null!;
}
