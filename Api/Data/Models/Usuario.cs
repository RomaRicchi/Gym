using System;
using System.Collections.Generic;

namespace Api.Data.Models
{
    public partial class Usuario
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string? Alias { get; set; }

        public int RolId { get; set; }

        public int? PersonalId { get; set; }

        public int? SocioId { get; set; }

        public string PasswordHash { get; set; } = null!;

        public bool Estado { get; set; } // 1 = activo, 0 = inactivo, -1 = eliminado    

        public DateTime CreadoEn { get; set; }

        public int? IdAvatar { get; set; }  // 🔹 nuevo campo opcional

        // 🔹 Relaciones
        public virtual Rol Rol { get; set; } = null!;

        public virtual Personal? Personal { get; set; }

        public virtual Socio? Socio { get; set; }

        public virtual Avatar? Avatar { get; set; } // 🔹 navegación hacia Avatar
    }
}
