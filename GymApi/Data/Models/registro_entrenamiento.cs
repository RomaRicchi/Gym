using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;


public partial class registro_entrenamiento
{
    public uint id { get; set; }

    public uint rutina_asignada_id { get; set; }

    public DateOnly fecha { get; set; }

    public string? observaciones { get; set; }

    public virtual ICollection<registro_item> registro_item { get; set; } = new List<registro_item>();

    public virtual rutina_asignada rutina_asignada { get; set; } = null!;
}
