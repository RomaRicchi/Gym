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

        // 🔹 Listar todos los estados
        public async Task<IReadOnlyList<EstadoOrdenPago>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.EstadoOrdenPago
                .OrderBy(e => e.Id)
                .ToListAsync(ct);
        }

        // 🔹 Buscar por ID
        public async Task<EstadoOrdenPago?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.EstadoOrdenPago
                .FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        // 🔹 Buscar por nombre (ej: "pendiente", "verificado")
        public async Task<EstadoOrdenPago?> GetByNombreAsync(string nombre, CancellationToken ct = default)
        {
            return await _db.EstadoOrdenPago
                .FirstOrDefaultAsync(e => e.Nombre == nombre, ct);
        }

        // 🔹 Agregar un nuevo estado
        public async Task<EstadoOrdenPago> AddAsync(EstadoOrdenPago entity, CancellationToken ct = default)
        {
            _db.EstadoOrdenPago.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        // 🔹 Actualizar un estado existente
        public async Task UpdateAsync(EstadoOrdenPago entity, CancellationToken ct = default)
        {
            _db.EstadoOrdenPago.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        // 🔹 Eliminar un estado
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
