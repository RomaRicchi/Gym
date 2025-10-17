using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class OrdenPagoRepository : IOrdenPagoRepository
    {
        private readonly GymDbContext _db;

        public OrdenPagoRepository(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 Obtener una orden por ID (incluye estado, socio, plan y comprobante)
        public async Task<OrdenPago?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.OrdenesPago
                .Include(o => o.Estado)
                .Include(o => o.Socio)
                .Include(o => o.Plan)
                .Include(o => o.Comprobante)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id, ct);
        }

        // 🔹 Listar todas las órdenes
        public async Task<IReadOnlyList<OrdenPago>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.OrdenesPago
                .Include(o => o.Estado)
                .Include(o => o.Socio)
                .Include(o => o.Plan)
                .Include(o => o.Comprobante) 
                .OrderByDescending(o => o.CreadoEn)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        // 🔹 Listar por nombre de estado (ej: "pendiente", "verificado")
        public async Task<IReadOnlyList<OrdenPago>> GetByEstadoAsync(string estadoNombre, CancellationToken ct = default)
        {
            return await _db.OrdenesPago
                .Include(o => o.Estado)
                .Include(o => o.Socio)
                .Include(o => o.Plan)
                .Include(o => o.Comprobante)
                .Where(o => o.Estado.Nombre == estadoNombre)
                .OrderByDescending(o => o.CreadoEn)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        // 🔹 Agregar una nueva orden
        public async Task<OrdenPago> AddAsync(OrdenPago entity, CancellationToken ct = default)
        {
            // 🧩 Evitar referencias cíclicas innecesarias
            entity.Socio = null!;
            entity.Plan = null!;
            entity.Estado = null!;
            entity.Comprobante = null;

            _db.OrdenesPago.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        // 🔹 Actualizar una orden existente
        public async Task UpdateAsync(OrdenPago entity, CancellationToken ct = default)
        {
            _db.OrdenesPago.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        // 🔹 Eliminar una orden (baja física)
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var orden = await _db.OrdenesPago.FindAsync(new object?[] { id }, ct);
            if (orden != null)
            {
                _db.OrdenesPago.Remove(orden);
                await _db.SaveChangesAsync(ct);
            }
        }

        // 🔹 Listar órdenes que aún no generaron suscripción
        public async Task<IReadOnlyList<OrdenPago>> GetPendientesDeSuscripcionAsync(CancellationToken ct = default)
        {
            return await _db.OrdenesPago
                .Include(o => o.Estado)
                .Include(o => o.Socio)
                .Include(o => o.Plan)
                .Include(o => o.Comprobante)
                .Where(o => !_db.Suscripciones.Any(s => s.OrdenPagoId == o.Id))
                .OrderByDescending(o => o.CreadoEn)
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
