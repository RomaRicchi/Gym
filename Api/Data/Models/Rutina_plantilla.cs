using System;
using System.Collections.Generic;

namespace Api.Data.Models;


public partial class Rutina_plantilla
{
    public uint Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Objetivo { get; set; }

    public uint? Plan_id { get; set; }

    public virtual plan? Plan { get; set; }

    public virtual ICollection<rutina_asignada> Rutina_asignada { get; set; } = new List<rutina_asignada>();

    public virtual ICollection<rutina_plantilla_ejercicio> Rutina_plantilla_ejercicio { get; set; } = new List<rutina_plantilla_ejercicio>();
}
