using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class RegistroEntrenamiento
{
    public uint Id { get; set; }

    public uint RutinaAsignadaId { get; set; }

    public DateOnly Fecha { get; set; }

    public string? Observaciones { get; set; }

    public virtual ICollection<RegistroItem> RegistrosItem { get; set; } = new List<RegistroItem>();

    public virtual RutinaAsignada RutinaAsignada { get; set; } = null!;
}
