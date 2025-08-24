using System;
using System.Collections.Generic;
namespace GymApi.Data.Models;


public partial class suscripcion_turno
{
    public uint id { get; set; }

    public uint suscripcion_id { get; set; }

    public uint turno_plantilla_id { get; set; }

    public virtual suscripcion suscripcion { get; set; } = null!;

    public virtual turno_plantilla turno_plantilla { get; set; } = null!;
}
