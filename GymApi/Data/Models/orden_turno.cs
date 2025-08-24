using System;
using System.Collections.Generic;
namespace GymApi.Data.Models;

public partial class orden_turno
{
    public uint id { get; set; }

    public uint orden_id { get; set; }

    public uint turno_plantilla_id { get; set; }

    public virtual orden_pago orden { get; set; } = null!;

    public virtual turno_plantilla turno_plantilla { get; set; } = null!;
}
