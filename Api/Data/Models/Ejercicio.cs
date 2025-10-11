using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class Ejercicio
{
    public uint Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Grupo { get; set; }

    public string? Tips { get; set; }

    public string? MediaUrl { get; set; }

    public virtual ICollection<RegistroItem> RegistroItems { get; set; } = new List<RegistroItem>();

    public virtual ICollection<RutinaPlantillaEjercicio> RutinaPlantillaEjercicios { get; set; } = new List<RutinaPlantillaEjercicio>();
}
