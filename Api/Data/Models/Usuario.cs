using System;
using System.Collections.Generic;

namespace Api.Data.Models
{
    public partial class Usuario
    {
        public uint Id { get; set; }

        public string Email { get; set; } = null!;

        public string? Alias { get; set; }

        public uint RolId { get; set; }

        public uint? PersonalId { get; set; }

        public uint? SocioId { get; set; }

        public string PasswordHash { get; set; } = null!;
        
        public sbyte Estado { get; set; }  // 1 = activo, 0 = inactivo, -1 = eliminado    
        
        public DateTime CreadoEn { get; set; }

        // 🔹 Relaciones
        public virtual Rol Rol { get; set; } = null!;

        public virtual Personal? Personal { get; set; }

        public virtual Socio? Socio { get; set; }
    }
}
