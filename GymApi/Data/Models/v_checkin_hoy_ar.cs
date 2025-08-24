using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class v_checkin_hoy_ar
{
    public uint id { get; set; }

    public uint socio_id { get; set; }

    public uint? turno_plantilla_id { get; set; }

    public DateTime? fecha_ar { get; set; }

    public string origen { get; set; } = null!;
}
