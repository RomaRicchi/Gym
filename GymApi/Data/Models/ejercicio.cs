using System;
using System.Collections.Generic;

namespace GymApi.Data.Models;

public partial class ejercicio
{
    public uint id { get; set; }

    public string nombre { get; set; } = null!;

    public string? grupo { get; set; }

    public string? tips { get; set; }

    public string? media_url { get; set; }

    public virtual ICollection<registro_item> registro_item { get; set; } = new List<registro_item>();

    public virtual ICollection<rutina_plantilla_ejercicio> rutina_plantilla_ejercicio { get; set; } = new List<rutina_plantilla_ejercicio>();
}
