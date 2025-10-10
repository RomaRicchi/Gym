using System;
using System.Collections.Generic;

namespace Api.Data.Models;


public partial class V_ordenes_ar
{
    public uint Id { get; set; }

    public uint Socio_id { get; set; }

    public uint Plan_id { get; set; }

    public decimal Monto { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime? Vence_en_ar { get; set; }

    public DateTime Creado_en { get; set; }
}
