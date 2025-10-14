using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class RegistroItem
{
    public int Id { get; set; }

    public int RegistroId { get; set; }

    public int? EjercicioId { get; set; }

    public int? Series { get; set; }

    public int? Repeticiones { get; set; }

    public decimal? Carga { get; set; }

    public virtual Ejercicio? Ejercicio { get; set; }

    public virtual RegistroEntrenamiento Registro { get; set; } = null!;
}
