using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class RolRepository : IRolRepository
    {
        private readonly GymDbContext _db;

        public RolRepository(GymDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<Rol>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Roles
                .AsNoTracking()
                .OrderBy(r => r.Id)
                .ToListAsync(ct);
        }

        public async Task<Rol?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id, ct);
        }

        public async Task<Rol> AddAsync(Rol rol, CancellationToken ct = default)
        {
            _db.Roles.Add(rol);
            await _db.SaveChangesAsync(ct);
            return rol;
        }

        public async Task<Rol?> UpdateAsync(int id, Rol rol, CancellationToken ct = default)
        {
            var existing = await _db.Roles.FindAsync(new object[] { id }, ct);
            if (existing == null) return null;

            existing.Nombre = rol.Nombre;
            await _db.SaveChangesAsync(ct);
            return existing;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var rol = await _db.Roles.FindAsync(new object[] { id }, ct);
            if (rol == null) return false;

            _db.Roles.Remove(rol);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
