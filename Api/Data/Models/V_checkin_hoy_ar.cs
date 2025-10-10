using System;
using System.Collections.Generic;

namespace Api.Data.Models;


public partial class V_checkin_hoy_ar
{
    public uint Id { get; set; }

    public uint Socio_id { get; set; }

    public uint? Turno_plantilla_id { get; set; }

    public DateTime? Fecha_ar { get; set; }

    public string Origen { get; set; } = null!;
}
