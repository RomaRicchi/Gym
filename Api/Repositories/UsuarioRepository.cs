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

        public async Task<IReadOnlyList<Usuario>> GetAllAsync(CancellationToken ct = default)
        {
            // 🔹 Solo usuarios activos
            return await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Avatar)
                .AsNoTracking()
                .Where(u => u.Estado == 1)
                .OrderBy(u => u.Id)
                .ToListAsync(ct);
        }

        public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Avatar)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && u.Estado == 1, ct);
        }

        public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Avatar)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && u.Estado == 1, ct);
        }

        public async Task<Usuario> AddAsync(Usuario usuario, CancellationToken ct = default)
        {
            usuario.CreadoEn = DateTime.UtcNow;
            usuario.Estado = 1; // activo por defecto
            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync(ct);
            return usuario;
        }

        public async Task<bool> UpdateAsync(Usuario usuario, CancellationToken ct = default)
        {
            var existing = await _db.Usuarios.FindAsync(new object[] { usuario.Id }, ct);
            if (existing == null) return false;

            existing.Email = usuario.Email;
            existing.Alias = usuario.Alias;
            existing.RolId = usuario.RolId;
            existing.PersonalId = usuario.PersonalId;
            existing.SocioId = usuario.SocioId;
            existing.Estado = usuario.Estado;
            existing.IdAvatar = usuario.IdAvatar;

            await _db.SaveChangesAsync(ct);
            return true;
        }

        // 🔹 BAJA LÓGICA (Estado = 0)
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var usuario = await _db.Usuarios.FindAsync(new object[] { id }, ct);
            if (usuario == null) return false;

            usuario.Estado = 0;
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> UpdatePerfilAsync(int id, string? nombre, string? email, string? telefono, int? idAvatar, CancellationToken ct = default)
        {
            var user = await _db.Usuarios
                .Include(u => u.Socio)
                .Include(u => u.Personal)
                .FirstOrDefaultAsync(u => u.Id == id, ct);

            if (user is null)
                return false;

            if (!string.IsNullOrWhiteSpace(email))
                user.Email = email.Trim();

            if (idAvatar.HasValue)
                user.IdAvatar = idAvatar.Value;

            if (user.SocioId.HasValue && user.Socio != null)
            {
                if (!string.IsNullOrWhiteSpace(nombre))
                    user.Socio.Nombre = nombre.Trim();
                if (!string.IsNullOrWhiteSpace(telefono))
                    user.Socio.Telefono = telefono.Trim();
            }

            if (user.PersonalId.HasValue && user.Personal != null)
            {
                if (!string.IsNullOrWhiteSpace(nombre))
                    user.Personal.Nombre = nombre.Trim();
                if (!string.IsNullOrWhiteSpace(telefono))
                    user.Personal.Telefono = telefono.Trim();
            }

            await _db.SaveChangesAsync(ct);
            return true;
        }


    }
}
