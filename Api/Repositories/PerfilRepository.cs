using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Api.Repositories
{
    public class PerfilRepository : IPerfilRepository
    {
        private readonly GymDbContext _db;

        public PerfilRepository(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 GET perfil básico
        public async Task<Usuario?> GetPerfilAsync(int id, CancellationToken ct)
        {
            return await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Personal)
                .Include(u => u.Socio)
                .Include(u => u.Avatar)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        // 🔹 GET perfil detallado
        public async Task<Usuario?> GetPerfilDetalladoAsync(int id, CancellationToken ct)
        {
            return await _db.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Personal)
                .Include(u => u.Socio)
                .Include(u => u.Avatar)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        // 🔹 PUT actualizar datos del perfil
        public async Task<bool> UpdatePerfilAsync(int id, string? nombre, string? email, string? telefono, int? idAvatar, CancellationToken ct)
        {
            var user = await _db.Usuarios
                .Include(u => u.Socio)
                .Include(u => u.Personal)
                .FirstOrDefaultAsync(u => u.Id == id, ct);

            if (user is null)
                return false;

            user.Email = email ?? user.Email;
            user.IdAvatar = idAvatar ?? user.IdAvatar;

            if (user.Socio != null)
            {
                if (!string.IsNullOrEmpty(nombre)) user.Socio.Nombre = nombre;
                if (!string.IsNullOrEmpty(telefono)) user.Socio.Telefono = telefono;
            }

            if (user.Personal != null)
            {
                if (!string.IsNullOrEmpty(nombre)) user.Personal.Nombre = nombre;
                if (!string.IsNullOrEmpty(telefono)) user.Personal.Telefono = telefono;
            }

            await _db.SaveChangesAsync(ct);
            return true;
        }

        // 🔹 POST subir o asignar nuevo avatar
        public async Task<Avatar> SubirAvatarAsync(int id, Avatar avatar, CancellationToken ct)
        {
            var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user is null)
                throw new Exception("Usuario no encontrado.");

            _db.Avatares.Add(avatar);
            await _db.SaveChangesAsync(ct);

            user.IdAvatar = avatar.Id;
            await _db.SaveChangesAsync(ct);

            return avatar;
        }

        // 🔹 PATCH cambiar contraseña
        public async Task<bool> CambiarPasswordAsync(int id, string passwordActual, string nuevaPassword, CancellationToken ct)
        {
            var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);
            if (user is null)
                return false;

            var hashActual = CalcularHash(passwordActual);
            if (user.PasswordHash != hashActual)
                return false;

            user.PasswordHash = CalcularHash(nuevaPassword);
            await _db.SaveChangesAsync(ct);

            return true;
        }

        // 🔐 Utilidad privada para calcular hash SHA256
        private static string CalcularHash(string texto)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(texto);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
