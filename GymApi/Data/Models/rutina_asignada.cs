using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class rutina_asignada
{
    public uint id { get; set; }

    public uint socio_id { get; set; }

    public uint rutina_id { get; set; }

    public DateOnly inicio { get; set; }

    public DateOnly? fin { get; set; }

    public string? notas { get; set; }

    public virtual ICollection<registro_entrenamiento> registro_entrenamiento { get; set; } = new List<registro_entrenamiento>();

    public virtual rutina_plantilla rutina { get; set; } = null!;

    public virtual socio socio { get; set; } = null!;
}
