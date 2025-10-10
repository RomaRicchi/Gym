using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class Checkin
{
    public uint Id { get; set; }

    public uint Socio_id { get; set; }

    public uint? Turno_plantilla_id { get; set; }

    public DateTime Fecha_hora { get; set; }

    public string Origen { get; set; } = null!;

    public virtual socio Socio { get; set; } = null!;

    public virtual turno_plantilla? Turno_plantilla { get; set; }
}
