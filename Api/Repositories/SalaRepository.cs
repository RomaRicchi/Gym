using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class SalaRepository : ISalaRepository
    {
        private readonly GymDbContext _db;

        public SalaRepository(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 Listar todas las salas
        public async Task<IReadOnlyList<Sala>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Salas
                .OrderBy(s => s.Nombre)
                .ToListAsync(ct);
        }

        // 🔹 Buscar una sala por ID
        public async Task<Sala?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Salas
                .FirstOrDefaultAsync(s => s.Id == id, ct);
        }

        // 🔹 Crear una nueva sala
        public async Task<Sala> AddAsync(Sala entity, CancellationToken ct = default)
        {
            _db.Salas.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        // 🔹 Actualizar una sala existente
        public async Task UpdateAsync(Sala entity, CancellationToken ct = default)
        {
            _db.Salas.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        // 🔹 Eliminar una sala
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var sala = await _db.Salas.FindAsync(new object?[] { id }, ct);
            if (sala != null)
            {
                _db.Salas.Remove(sala);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
