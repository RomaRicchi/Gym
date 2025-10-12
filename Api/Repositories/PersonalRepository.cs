using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class PersonalRepository : IPersonalRepository
    {
        private readonly GymDbContext _db;

        public PersonalRepository(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 Listar todo el personal
        public async Task<IReadOnlyList<Personal>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Personales
                .OrderBy(p => p.Nombre)
                .ToListAsync(ct);
        }

        // 🔹 Listar solo los activos
        public async Task<IReadOnlyList<Personal>> GetActivosAsync(CancellationToken ct = default)
        {
            return await _db.Personales
                .Where(p => p.Estado)
                .OrderBy(p => p.Nombre)
                .ToListAsync(ct);
        }

        // 🔹 Obtener por ID
        public async Task<Personal?> GetByIdAsync(uint id, CancellationToken ct = default)
        {
            return await _db.Personales
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        // 🔹 Agregar nuevo personal
        public async Task<Personal> AddAsync(Personal entity, CancellationToken ct = default)
        {
            _db.Personales.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        // 🔹 Actualizar datos del personal
        public async Task UpdateAsync(Personal entity, CancellationToken ct = default)
        {
            _db.Personales.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        // 🔹 Eliminar (soft delete o real delete)
        public async Task DeleteAsync(uint id, CancellationToken ct = default)
        {
            var personal = await _db.Personales.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (personal != null)
            {
                _db.Personales.Remove(personal);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
