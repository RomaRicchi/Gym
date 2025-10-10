using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class Orden_pago
{
    public uint Id { get; set; }

    public uint Socio_id { get; set; }

    public uint Plan_id { get; set; }

    public decimal Monto { get; set; }

    public DateTime Vence_en { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime Creado_en { get; set; }

    public string? Notas { get; set; }

    public virtual ICollection<comprobante> Comprobantes { get; set; } = new List<comprobante>();

    public virtual ICollection<orden_turno> Orden_turnos { get; set; } = new List<orden_turno>();

    public virtual plan Plan { get; set; } = null!;

    public virtual socio Socio { get; set; } = null!;

    
        // ✅ Relación 1:1 con comprobante
    public comprobante Comprobante { get; set; }

        // ✅ Relación 1:N con orden_turno
    public ICollection<orden_turno> Orden_turno { get; set; }
}
