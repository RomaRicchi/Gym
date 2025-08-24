using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;

public partial class checkin
{
    public uint id { get; set; }

    public uint socio_id { get; set; }

    public uint? turno_plantilla_id { get; set; }

    public DateTime fecha_hora { get; set; }

    public string origen { get; set; } = null!;

    public virtual socio socio { get; set; } = null!;

    public virtual turno_plantilla? turno_plantilla { get; set; }
}
