using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class ComprobanteRepository : IComprobanteRepository
    {
        private readonly GymDbContext _db;

        public ComprobanteRepository(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 Obtener comprobante por ID (con su orden)
        public async Task<Comprobante?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Comprobantes
                .Include(c => c.OrdenPago)
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        // 🔹 Obtener comprobante vinculado a una orden (1:1)
        public async Task<Comprobante?> GetByOrdenAsync(int ordenId, CancellationToken ct = default)
        {
            var orden = await _db.OrdenesPago
                .Include(o => o.Comprobante)
                .FirstOrDefaultAsync(o => o.Id == ordenId, ct);

            return orden?.Comprobante;
        }

        // 🔹 Agregar nuevo comprobante y asociarlo a la orden
        public async Task<Comprobante> AddAsync(Comprobante entity, int ordenId, CancellationToken ct = default)
        {
            // 1️⃣ Guardamos el comprobante
            _db.Comprobantes.Add(entity);
            await _db.SaveChangesAsync(ct);

            // 2️⃣ Asociamos el comprobante a la orden
            var orden = await _db.OrdenesPago.FindAsync(new object[] { ordenId }, ct);
            if (orden != null)
            {
                orden.ComprobanteId = entity.Id;
                _db.OrdenesPago.Update(orden);
                await _db.SaveChangesAsync(ct);
            }

            return entity;
        }

        // 🔹 Eliminar comprobante (y desasociar la orden)
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var comp = await _db.Comprobantes.FindAsync(new object?[] { id }, ct);
            if (comp != null)
            {
                // Desvincular de la orden si corresponde
                var orden = await _db.OrdenesPago.FirstOrDefaultAsync(o => o.ComprobanteId == id, ct);
                if (orden != null)
                {
                    orden.ComprobanteId = null;
                    _db.OrdenesPago.Update(orden);
                }

                _db.Comprobantes.Remove(comp);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
