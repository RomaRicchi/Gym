using System.Collections.Generic;

namespace Api.Data.Models
{
    public partial class Avatar
    {
        public int Id { get; set; }

        public string Url { get; set; } = null!;

        public string? Nombre { get; set; }

        public bool EsPredeterminado { get; set; }

        // 🔹 Relación inversa
        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
