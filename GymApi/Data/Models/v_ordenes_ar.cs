using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class v_ordenes_ar
{
    public uint id { get; set; }

    public uint socio_id { get; set; }

    public uint plan_id { get; set; }

    public decimal monto { get; set; }

    public string estado { get; set; } = null!;

    public DateTime? vence_en_ar { get; set; }

    public DateTime creado_en { get; set; }
}
