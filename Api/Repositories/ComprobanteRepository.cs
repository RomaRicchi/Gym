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

        // 🔹 1️⃣ Obtener comprobante por ID (incluye su orden)
        public async Task<Comprobante?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Comprobantes
                .Include(c => c.OrdenPago)
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        // 🔹 2️⃣ Obtener comprobante asociado a una orden (relación 1:1)
        public async Task<Comprobante?> GetByOrdenAsync(int ordenId, CancellationToken ct = default)
        {
            var orden = await _db.OrdenesPago
                .Include(o => o.Comprobante)
                .FirstOrDefaultAsync(o => o.Id == ordenId, ct);

            return orden?.Comprobante;
        }

        // 🔹 3️⃣ Crear nuevo comprobante (sin asociar)
        public async Task<Comprobante> AddAsync(Comprobante entity, CancellationToken ct = default)
        {
            await _db.Comprobantes.AddAsync(entity, ct);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        // 🔹 4️⃣ Eliminar comprobante y desasociar su orden si corresponde
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var comp = await _db.Comprobantes.FindAsync(new object?[] { id }, ct);
            if (comp != null)
            {
                // Si hay una orden vinculada, la desvinculamos
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
