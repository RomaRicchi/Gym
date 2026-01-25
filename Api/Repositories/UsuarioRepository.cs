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

        // 🔹 Obtener todos los usuarios con su rol y avatar
        public async Task<IReadOnlyList<object>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Avatar)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Alias,
                    Rol = u.Rol != null ? u.Rol.Nombre : "(Sin rol)",
                    AvatarUrl = u.Avatar != null ? u.Avatar.Url : null,
                    u.Estado
                })
                .ToListAsync(ct);
        }

        // 🔹 Obtener un usuario por ID
        public async Task<object?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Avatar)
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Alias,
                    Rol = u.Rol != null ? u.Rol.Nombre : "(Sin rol)",
                    AvatarUrl = u.Avatar != null ? u.Avatar.Url : null,
                    u.Estado
                })
                .FirstOrDefaultAsync(ct);
        }

        // 🔹 Crear usuario
        public async Task<Usuario> CreateAsync(Usuario u, CancellationToken ct = default)
        {
            _db.Usuarios.Add(u);
            await _db.SaveChangesAsync(ct);
            return u;
        }

        // 🔹 Actualizar usuario
        public async Task<bool> UpdateAsync(int id, Usuario updated, CancellationToken ct = default)
        {
            var existing = await _db.Usuarios.FindAsync(new object[] { id }, ct);
            if (existing == null) return false;

            existing.Email = updated.Email;
            existing.Alias = updated.Alias;
            existing.RolId = updated.RolId;
            existing.Estado = updated.Estado;
            existing.IdAvatar = updated.IdAvatar;

            await _db.SaveChangesAsync(ct);
            return true;
        }

        // 🔹 Eliminar usuario
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var existing = await _db.Usuarios.FindAsync(new object[] { id }, ct);
            if (existing == null) return false;

            _db.Usuarios.Remove(existing);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
