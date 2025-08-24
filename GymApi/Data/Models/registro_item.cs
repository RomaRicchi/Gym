using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class registro_item
{
    public uint id { get; set; }

    public uint registro_id { get; set; }

    public uint? ejercicio_id { get; set; }

    public int? series { get; set; }

    public int? repeticiones { get; set; }

    public decimal? carga { get; set; }

    public virtual ejercicio? ejercicio { get; set; }

    public virtual registro_entrenamiento registro { get; set; } = null!;
}
