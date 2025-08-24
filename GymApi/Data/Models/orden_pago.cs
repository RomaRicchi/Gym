using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;

public partial class orden_pago
{
    public uint id { get; set; }

    public uint socio_id { get; set; }

    public uint plan_id { get; set; }

    public decimal monto { get; set; }

    public DateTime vence_en { get; set; }

    public string estado { get; set; } = null!;

    public DateTime creado_en { get; set; }

    public string? notas { get; set; }

    public virtual ICollection<comprobante> comprobante { get; set; } = new List<comprobante>();

    public virtual ICollection<orden_turno> orden_turno { get; set; } = new List<orden_turno>();

    public virtual plan plan { get; set; } = null!;

    public virtual socio socio { get; set; } = null!;
}
