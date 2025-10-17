using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class SuscripcionRepository : ISuscripcionRepository
    {
        private readonly GymDbContext _db;

        public SuscripcionRepository(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 Obtener todas las suscripciones
        public async Task<IReadOnlyList<Suscripcion>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Suscripciones
                .Include(s => s.Socio)
                .Include(s => s.Plan)
                .Include(s => s.OrdenPago) // ✅ nueva relación
                .OrderByDescending(s => s.CreadoEn)
                .ToListAsync(ct);
        }

        // 🔹 Solo las activas (estado = true)
        public async Task<IReadOnlyList<Suscripcion>> GetActivasAsync(CancellationToken ct = default)
        {
            return await _db.Suscripciones
                .Include(s => s.Socio)
                .Include(s => s.Plan)
                .Include(s => s.OrdenPago)
                .Where(s => s.Estado)
                .OrderByDescending(s => s.Inicio)
                .ToListAsync(ct);
        }

        // 🔹 Obtener suscripciones de un socio
        public async Task<IReadOnlyList<Suscripcion>> GetBySocioAsync(int socioId, CancellationToken ct = default)
        {
            return await _db.Suscripciones
                .Include(s => s.Plan)
                .Include(s => s.OrdenPago)
                .Where(s => s.SocioId == socioId)
                .OrderByDescending(s => s.Inicio)
                .ToListAsync(ct);
        }

       

        // 🔹 Buscar suscripción activa por socio y plan
        public async Task<Suscripcion?> GetActivaByPlanAsync(int socioId, int planId, CancellationToken ct = default)
        {
            return await _db.Suscripciones
                .Include(s => s.OrdenPago)
                .Where(s => s.SocioId == socioId && s.PlanId == planId && s.Estado)
                .OrderByDescending(s => s.Inicio)
                .FirstOrDefaultAsync(ct);
        }

        // ✅ Nueva: buscar por ID de orden de pago
        public async Task<Suscripcion?> GetByOrdenPagoAsync(int ordenPagoId, CancellationToken ct = default)
        {
            return await _db.Suscripciones
                .Include(s => s.Socio)
                .Include(s => s.Plan)
                .Include(s => s.OrdenPago)
                .FirstOrDefaultAsync(s => s.OrdenPagoId == ordenPagoId, ct);
        }

        // 🔹 Agregar nueva suscripción
        public async Task<Suscripcion> AddAsync(Suscripcion entity, CancellationToken ct = default)
        {
            _db.Suscripciones.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        // 🔹 Actualizar
        public async Task UpdateAsync(Suscripcion entity, CancellationToken ct = default)
        {
            _db.Suscripciones.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        // 🔹 Eliminar
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var sus = await _db.Suscripciones.FindAsync(new object?[] { id }, ct);
            if (sus != null)
            {
                _db.Suscripciones.Remove(sus);
                await _db.SaveChangesAsync(ct);
            }
        }

        // 🔹 Exponer consulta base para filtrados avanzados
        public IQueryable<Suscripcion> Query()
        {
            return _db.Suscripciones
                .Include(s => s.Socio)
                .Include(s => s.Plan)
                .Include(s => s.OrdenPago)
                .AsQueryable();
        }

        public async Task<Suscripcion?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Suscripciones
                .Include(s => s.Socio)
                .Include(s => s.Plan)
                .Include(s => s.OrdenPago) 
                .FirstOrDefaultAsync(s => s.Id == id, ct);
        }

    }
}
