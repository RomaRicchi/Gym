using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class rutina_plantilla_ejercicio
{
    public uint id { get; set; }

    public uint rutina_id { get; set; }

    public uint ejercicio_id { get; set; }

    public int orden { get; set; }

    public int? series { get; set; }

    public int? repeticiones { get; set; }

    public int? descanso_seg { get; set; }

    public virtual ejercicio ejercicio { get; set; } = null!;

    public virtual rutina_plantilla rutina { get; set; } = null!;
}
