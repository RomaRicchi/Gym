using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Api.Contracts.Dtos;

namespace Api.Repositories
{
    public class RutinaPlantillaEjercicioRepository : IRutinaPlantillaEjercicioRepository
    {
        private readonly GymDbContext _db;

        public RutinaPlantillaEjercicioRepository(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 Obtener todos los registros
        public async Task<IEnumerable<RutinaPlantillaEjercicioDto>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.RutinasPlantillaEjercicios
                .Include(rpe => rpe.RutinaPlantilla)
                .Include(rpe => rpe.Ejercicio)
                .Select(rpe => new RutinaPlantillaEjercicioDto
                {
                    Id = rpe.Id,
                    RutinaId = rpe.RutinaId,
                    RutinaNombre = rpe.RutinaPlantilla.Nombre,
                    EjercicioId = rpe.EjercicioId,
                    EjercicioNombre = rpe.Ejercicio.Nombre,
                    Orden = rpe.Orden,
                    Series = rpe.Series,
                    Repeticiones = rpe.Repeticiones,
                    DescansoSeg = rpe.DescansoSeg
                })
                .AsNoTracking()
                .ToListAsync(ct);
        }

        // 🔹 Obtener un registro por ID
        public async Task<RutinaPlantillaEjercicio?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.RutinasPlantillaEjercicios
                .Include(rpe => rpe.Ejercicio)
                .Include(rpe => rpe.RutinaPlantilla)
                .AsNoTracking()
                .FirstOrDefaultAsync(rpe => rpe.Id == id, ct);
        }

        // 🔹 Obtener todos los ejercicios de una rutina específica
        public async Task<IReadOnlyList<RutinaPlantillaEjercicio>> GetByRutinaIdAsync(int rutinaId, CancellationToken ct = default)
        {
            return await _db.RutinasPlantillaEjercicios
                .Include(rpe => rpe.Ejercicio)
                .Include(rpe => rpe.RutinaPlantilla)
                .Where(rpe => rpe.RutinaId == rutinaId)
                .OrderBy(rpe => rpe.Orden)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<RutinaPlantillaEjercicio> AddAsync(RutinaPlantillaEjercicio entity, CancellationToken ct = default)
        {
            try
            {
                _db.RutinasPlantillaEjercicios.Add(entity);
                await _db.SaveChangesAsync(ct);
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al guardar RutinaPlantillaEjercicio: {ex.Message}");
                throw;
            }
        }

        // 🔹 Actualizar un registro existente
        public async Task<RutinaPlantillaEjercicio?> UpdateAsync(int id, RutinaPlantillaEjercicio entity, CancellationToken ct = default)
        {
            var existing = await _db.RutinasPlantillaEjercicios.FindAsync(new object[] { id }, ct);
            if (existing == null) return null;

            existing.RutinaId = entity.RutinaId;
            existing.EjercicioId = entity.EjercicioId;
            existing.Orden = entity.Orden;
            existing.Series = entity.Series;
            existing.Repeticiones = entity.Repeticiones;
            existing.DescansoSeg = entity.DescansoSeg;

            await _db.SaveChangesAsync(ct);
            return existing;
        }

        // 🔹 Eliminar un registro
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var existing = await _db.RutinasPlantillaEjercicios.FindAsync(new object[] { id }, ct);
            if (existing == null) return false;

            _db.RutinasPlantillaEjercicios.Remove(existing);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
