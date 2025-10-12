using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class OrdenTurnoRepository : IOrdenTurnoRepository
    {
        private readonly GymDbContext _db;

        public OrdenTurnoRepository(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 Listar todas las ordenes-turno
        public async Task<IReadOnlyList<OrdenTurno>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.OrdenesTurno
                .Include(o => o.OrdenPago)
                .Include(o => o.TurnoPlantilla)
                .OrderByDescending(o => o.CreadoEn)
                .ToListAsync(ct);
        }

        // 🔹 Obtener por ID
        public async Task<OrdenTurno?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.OrdenesTurno
                .Include(o => o.OrdenPago)
                .Include(o => o.TurnoPlantilla)
                .FirstOrDefaultAsync(o => o.Id == id, ct);
        }

        // 🔹 Listar todos los turnos asociados a una orden de pago
        public async Task<IReadOnlyList<OrdenTurno>> GetByOrdenPagoAsync(int ordenPagoId, CancellationToken ct = default)
        {
            return await _db.OrdenesTurno
                .Include(o => o.TurnoPlantilla)
                .Where(o => o.OrdenPagoId == ordenPagoId)
                .OrderBy(o => o.TurnoPlantilla.HoraInicio)
                .ToListAsync(ct);
        }

        // 🔹 Crear una nueva relación orden-turno
        public async Task<OrdenTurno> AddAsync(OrdenTurno entity, CancellationToken ct = default)
        {
            _db.OrdenesTurno.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        // 🔹 Actualizar un registro existente
        public async Task UpdateAsync(OrdenTurno entity, CancellationToken ct = default)
        {
            _db.OrdenesTurno.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        // 🔹 Eliminar un registro
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.OrdenesTurno.FindAsync(new object?[] { id }, ct);
            if (entity != null)
            {
                _db.OrdenesTurno.Remove(entity);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
