using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class RutinaAsignada
{
    public uint Id { get; set; }

    public uint SocioId { get; set; }

    public uint RutinaId { get; set; }

    public DateOnly Inicio { get; set; }

    public DateOnly? Fin { get; set; }

    public string? Notas { get; set; }

    public virtual ICollection<RegistroEntrenamiento> RegistrosEntrenamiento { get; set; } = new List<RegistroEntrenamiento>();

    public virtual RutinaPlantilla Rutina { get; set; } = null!;

    public virtual Socio Socio { get; set; } = null!;
}
