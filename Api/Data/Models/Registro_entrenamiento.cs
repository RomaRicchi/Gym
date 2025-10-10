using System;
using System.Collections.Generic;

namespace Api.Data.Models;


public partial class RegistroEntrenamiento
{
    public uint Id { get; set; }

    public uint Rutina_asignada_id { get; set; }

    public DateOnly Fecha { get; set; }

    public string? Observaciones { get; set; }

    public virtual ICollection<registro_item> Registro_item { get; set; } = new List<registro_item>();

    public virtual rutina_asignada Rutina_asignada { get; set; } = null!;
}
