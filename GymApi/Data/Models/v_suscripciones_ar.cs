using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class v_suscripciones_ar
{
    public uint id { get; set; }

    public uint socio_id { get; set; }

    public uint plan_id { get; set; }

    public DateTime? inicio_ar { get; set; }

    public DateTime? fin_ar { get; set; }

    public string estado { get; set; } = null!;

    public DateTime creado_en { get; set; }
}
