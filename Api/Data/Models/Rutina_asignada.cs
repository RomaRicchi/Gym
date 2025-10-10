using System;
using System.Collections.Generic;

namespace Api.Data.Models;


public partial class Rutina_asignada
{
    public uint Id { get; set; }

    public uint Socio_id { get; set; }

    public uint Rutina_id { get; set; }

    public DateOnly Inicio { get; set; }

    public DateOnly? Fin { get; set; }

    public string? Notas { get; set; }

    public virtual ICollection<registro_entrenamiento> Registro_entrenamiento { get; set; } = new List<registro_entrenamiento>();

    public virtual rutina_plantilla Rutina { get; set; } = null!;

    public virtual socio Socio { get; set; } = null!;
}
