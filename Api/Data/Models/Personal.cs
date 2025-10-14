using System;
using System.Collections.Generic;

namespace Api.Data.Models
{
    public partial class Personal
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        public string? Especialidad { get; set; }

        public bool Estado { get; set; }  // true = activo, false = inactivo

        // 🔹 Relaciones
        public virtual ICollection<TurnoPlantilla> TurnosPlantilla { get; set; } = new List<TurnoPlantilla>();
    }
}
