using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class RutinaPlantillaEjercicio
{
    public int Id { get; set; }

    public int RutinaId { get; set; }

    public int EjercicioId { get; set; }

    public int Orden { get; set; }

    public int? Series { get; set; }

    public int? Repeticiones { get; set; }

    public int? DescansoSeg { get; set; }

    public virtual Ejercicio Ejercicio { get; set; } = null!;

    public virtual RutinaPlantilla Rutina { get; set; } = null!;
}
