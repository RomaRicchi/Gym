using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly GymDbContext _db;

        public UsuarioRepository(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 Obtener todos los usuarios
        public async Task<IReadOnlyList<Usuario>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Personal)
                .Include(u => u.Socio)
                .OrderBy(u => u.Email)
                .ToListAsync(ct);
        }

        // 🔹 Obtener usuario por ID
        public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Personal)
                .Include(u => u.Socio)
                .FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        // 🔹 Obtener usuarios activos
        public async Task<IReadOnlyList<Usuario>> GetActivosAsync(CancellationToken ct = default)
        {
            return await _db.Usuarios
                .Where(u => u.Estado == true) // ✅ bool en lugar de int
                .Include(u => u.Rol)
                .ToListAsync(ct);
        }

        // 🔹 Buscar usuario por email (case insensitive)
        public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _db.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), ct);
        }

        // 🔹 Validar credenciales de login
        public async Task<Usuario?> LoginAsync(string email, string passwordHash, CancellationToken ct = default)
        {
            return await _db.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(
                    u => u.Email.ToLower() == email.ToLower()
                      && u.PasswordHash == passwordHash
                      && u.Estado == true, // ✅ bool
                    ct
                );
        }

        // 🔹 Crear nuevo usuario
        public async Task<Usuario> AddAsync(Usuario usuario, CancellationToken ct = default)
        {
            usuario.Estado = true; // ✅ bool
            usuario.CreadoEn = DateTime.UtcNow;

            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync(ct);
            return usuario;
        }

        // 🔹 Actualizar usuario existente
        public async Task<bool> UpdateAsync(Usuario usuario, CancellationToken ct = default)
        {
            var existing = await _db.Usuarios.FindAsync(new object[] { usuario.Id }, ct);
            if (existing == null) return false;

            existing.Email = usuario.Email;
            existing.Alias = usuario.Alias;
            existing.RolId = usuario.RolId;
            existing.PersonalId = usuario.PersonalId;
            existing.SocioId = usuario.SocioId;
            existing.PasswordHash = usuario.PasswordHash;
            existing.Estado = usuario.Estado; // ✅ bool
            existing.IdAvatar = usuario.IdAvatar;

            await _db.SaveChangesAsync(ct);
            return true;
        }

        // 🔹 Eliminar (baja lógica)
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var usuario = await _db.Usuarios.FindAsync(new object[] { id }, ct);
            if (usuario == null) return false;

            usuario.Estado = false; // ✅ bool
            await _db.SaveChangesAsync(ct);
            return true;
        }

        // 🔹 Existe un usuario con ese email
        public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null, CancellationToken ct = default)
        {
            var query = _db.Usuarios.AsQueryable();

            if (excludeId.HasValue)
                query = query.Where(u => u.Id != excludeId.Value);

            return await query.AnyAsync(u => u.Email.ToLower() == email.ToLower(), ct);
        }

        // 🔹 Cambiar estado activo/inactivo
        public async Task<bool> CambiarEstadoAsync(int id, bool nuevoEstado, CancellationToken ct = default)
        {
            var usuario = await _db.Usuarios.FindAsync(new object[] { id }, ct);
            if (usuario == null) return false;

            usuario.Estado = nuevoEstado; // ✅ bool
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
