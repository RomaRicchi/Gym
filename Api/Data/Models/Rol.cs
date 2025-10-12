using System;
using System.Collections.Generic;

namespace Api.Data.Models
{
    public partial class Rol
    {
        public uint Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        // 🔹 Relaciones
        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
