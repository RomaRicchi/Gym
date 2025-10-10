using System;
using System.Collections.Generic;

namespace Api.Data.Models;


public partial class Rutina_plantilla_ejercicio
{
    public uint Id { get; set; }

    public uint Rutina_id { get; set; }

    public uint Ejercicio_id { get; set; }

    public int Orden { get; set; }

    public int? Series { get; set; }

    public int? Repeticiones { get; set; }

    public int? Descanso_seg { get; set; }

    public virtual ejercicio Ejercicio { get; set; } = null!;
    public virtual rutina_plantilla Rutina { get; set; } = null!;
}
