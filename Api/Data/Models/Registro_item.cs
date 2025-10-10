using System;
using System.Collections.Generic;

namespace Api.Data.Models;


public partial class Registro_item
{
    public uint Id { get; set; }

    public uint Registro_id { get; set; }

    public uint? Ejercicio_id { get; set; }

    public int? Series { get; set; }

    public int? Repeticiones { get; set; }

    public decimal? Carga { get; set; }

    public virtual ejercicio? Ejercicio { get; set; }

    public virtual registro_entrenamiento Registro { get; set; } = null!;
}
