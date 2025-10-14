using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class EstadoOrdenPagoRepository : IEstadoOrdenPagoRepository
    {
        private readonly GymDbContext _db;

        public EstadoOrdenPagoRepository(GymDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<EstadoOrdenPago>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.EstadoOrdenPago
                .AsNoTracking()
                .OrderBy(e => e.Id)
                .ToListAsync(ct);
        }

        public Task<EstadoOrdenPago?> GetByIdAsync(int id, CancellationToken ct = default)
            => _db.EstadoOrdenPago.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
        public async Task<EstadoOrdenPago?> GetByNombreAsync(string nombre, CancellationToken ct = default)
        {
            var normalized = nombre.Trim().ToLower();

            return await _db.EstadoOrdenPago
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Nombre.ToLower().StartsWith(normalized), ct);
        }

        public async Task<EstadoOrdenPago> AddAsync(EstadoOrdenPago entity, CancellationToken ct = default)
        {
            _db.EstadoOrdenPago.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task UpdateAsync(EstadoOrdenPago entity, CancellationToken ct = default)
        {
            _db.EstadoOrdenPago.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        // 🔹 Eliminación física (sin campo activo)
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var estado = await _db.EstadoOrdenPago.FindAsync(new object?[] { id }, ct);
            if (estado != null)
            {
                _db.EstadoOrdenPago.Remove(estado);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
