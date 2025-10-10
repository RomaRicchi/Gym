using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class Ejercicio
{
    public uint Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Grupo { get; set; }

    public string? Tips { get; set; }

    public string? Media_url { get; set; }

    public virtual ICollection<registro_item> Registro_item { get; set; } = new List<registro_item>();

    public virtual ICollection<rutina_plantilla_ejercicio> Rutina_plantilla_ejercicio { get; set; } = new List<rutina_plantilla_ejercicio>();
}
