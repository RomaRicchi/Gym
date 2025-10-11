using System;
using System.Collections.Generic;

namespace Api.Data.Models;

public partial class Profesor
{
    public uint Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Email { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<TurnoPlantilla> TurnosPlantilla { get; set; } = new List<TurnoPlantilla>();
}
