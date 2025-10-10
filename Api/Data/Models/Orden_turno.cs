using System;
using System.Collections.Generic;
namespace Api.Data.Models;

public partial class Orden_turno
{
    public uint Id { get; set; }

    public uint Orden_id { get; set; }

    public uint Turno_plantilla_id { get; set; }

    public virtual orden_pago Orden_pago { get; set; } = null!;

    public virtual turno_plantilla Turno_plantilla { get; set; } = null!;
}
