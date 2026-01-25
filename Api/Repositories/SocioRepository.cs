using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Repositories
{
    public class SocioRepository : ISocioRepository
    {
        private readonly GymDbContext _db;

        public SocioRepository(GymDbContext db)
        {
            _db = db;
        }

        // ✅ Paginado con PlanActual calculado
        public async Task<(IReadOnlyList<Socio> items, int total)> GetPagedAsync(
            int page,
            int pageSize,
            string? q = null,
            bool? activo = null,
            CancellationToken ct = default)
        {
            var qry = _db.Socios
                .AsNoTracking()
                .Include(s => s.Suscripciones)
                    .ThenInclude(su => su.Plan)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                qry = qry.Where(s => s.Nombre.Contains(q) || s.Dni.Contains(q) || s.Email.Contains(q));

            if (activo.HasValue)
                qry = qry.Where(s => s.Activo == activo.Value);

            var total = await qry.CountAsync(ct);

            var items = await qry
                .OrderBy(s => s.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            // Calcular plan actual
            foreach (var socio in items)
            {
                socio.PlanActual = socio.Suscripciones
                    .Where(su =>
                        su.Inicio <= DateTime.UtcNow &&
                        su.Fin >= DateTime.UtcNow &&
                        su.Estado) // bool
                    .OrderByDescending(su => su.Inicio)
                    .Select(su => su.Plan.Nombre)
                    .FirstOrDefault();
            }

            return (items, total);
        }

        // ✅ Alta
        public async Task<Socio> AddAsync(Socio socio, CancellationToken ct = default)
        {
            _db.Socios.Add(socio);
            await _db.SaveChangesAsync(ct);
            return socio;
        }

        // ✅ Consulta por ID
        public async Task<Socio?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Socios
                .Include(s => s.Suscripciones)
                    .ThenInclude(su => su.Plan)
                .FirstOrDefaultAsync(s => s.Id == id, ct);
        }

        // ✅ Baja real (devuelve true si existía)
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var socio = await _db.Socios.FindAsync(new object[] { id }, ct);
            if (socio == null) return false;

            _db.Socios.Remove(socio);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        // ✅ Update con acción lambda (devuelve true si se encontró)
        public async Task<bool> UpdateAsync(int id, Action<Socio> updateAction, CancellationToken ct = default)
        {
            var socio = await _db.Socios.FindAsync(new object[] { id }, ct);
            if (socio == null) return false;

            updateAction(socio);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        // ✅ Verifica existencia por DNI o Email
        public async Task<bool> ExistsAsync(string dni, string email, CancellationToken ct = default)
        {
            return await _db.Socios.AnyAsync(s => s.Dni == dni || s.Email == email, ct);
        }

        // ✅ Cambiar activo/inactivo (devuelve true si se encontró)
        public async Task<bool> SetActivoAsync(int id, bool value, CancellationToken ct = default)
        {
            var socio = await _db.Socios.FindAsync(new object[] { id }, ct);
            if (socio == null) return false;

            socio.Activo = value;
            await _db.SaveChangesAsync(ct);
            return true;
        }

        // ✅ Retorna IQueryable para queries externas
        public IQueryable<Socio> Query()
        {
            return _db.Socios.AsQueryable();
        }
    }
}
