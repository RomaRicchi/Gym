using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace Api.Data.Models;

public partial class Sala
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int Cupo { get; set; }

    public bool? Activa { get; set; }

    public virtual ICollection<TurnoPlantilla> TurnosPlantilla { get; set; } = new List<TurnoPlantilla>();
    
    [NotMapped]
    public int CupoDisponible { get; set; }
}
