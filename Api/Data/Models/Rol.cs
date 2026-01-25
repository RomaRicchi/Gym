using System.Collections.Generic;

namespace Api.Data.Models
{
    public partial class Rol
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        // 🔹 Relaciones
        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
