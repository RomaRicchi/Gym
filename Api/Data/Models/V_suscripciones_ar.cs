using System;
using System.Collections.Generic;

namespace Api.Data.Models;


public partial class V_suscripciones_ar
{
    public uint Id { get; set; }

    public uint Socio_id { get; set; }

    public uint Plan_id { get; set; }

    public DateTime? Inicio_ar { get; set; }

    public DateTime? Fin_ar { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime Creado_en { get; set; }
}
