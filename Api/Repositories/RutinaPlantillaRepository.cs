using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Repositories
{
    public class RutinaPlantillaRepository : IRutinaPlantillaRepository
    {
        private readonly GymDbContext _db;

        public RutinaPlantillaRepository(GymDbContext db)
        {
            _db = db;
        }

        // ===============================
        // 🔹 GET todas las rutinas con grupo muscular y ejercicios
        // ===============================
        public async Task<IEnumerable<RutinaPlantilla>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.RutinasPlantilla
                .Include(r => r.GrupoMuscular)
                .Include(r => r.RutinaPlantillaEjercicios)
                    .ThenInclude(rpe => rpe.Ejercicio)
                .OrderBy(r => r.Nombre)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        // ===============================
        // 🔹 GET por ID (detalle completo)
        // ===============================
        public async Task<RutinaPlantilla?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.RutinasPlantilla
                .Include(r => r.GrupoMuscular)
                .Include(r => r.RutinaPlantillaEjercicios)
                    .ThenInclude(rpe => rpe.Ejercicio)
                .FirstOrDefaultAsync(r => r.Id == id, ct);
        }

        // ===============================
        // 🔹 POST (crear nueva rutina)
        // ===============================
        public async Task<RutinaPlantilla> AddAsync(RutinaPlantilla entity, CancellationToken ct = default)
        {
            _db.RutinasPlantilla.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        // ===============================
        // 🔹 PUT (actualizar rutina existente)
        // ===============================
        public async Task<RutinaPlantilla?> UpdateAsync(int id, RutinaPlantilla entity, CancellationToken ct = default)
        {
            var existing = await _db.RutinasPlantilla
                .Include(r => r.RutinaPlantillaEjercicios)
                .FirstOrDefaultAsync(r => r.Id == id, ct);

            if (existing == null)
                return null;

            // Actualiza propiedades principales
            existing.Nombre = entity.Nombre;
            existing.Objetivo = entity.Objetivo;
            existing.GrupoMuscularId = entity.GrupoMuscularId;
            existing.ImagenUrl = entity.ImagenUrl;

            // Si se quisieran actualizar los ejercicios, podrías hacerlo aquí manualmente
            // (por ahora, no tocamos la lista intermedia)

            await _db.SaveChangesAsync(ct);
            return existing;
        }

        // ===============================
        // 🔹 DELETE
        // ===============================
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.RutinasPlantilla.FindAsync(new object[] { id }, ct);
            if (entity == null)
                return false;

            _db.RutinasPlantilla.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<IEnumerable<RutinaPlantilla>> GetByGrupoMuscularAsync(int grupoMuscularId, CancellationToken ct)
        {
            return await _db.RutinasPlantilla
                .Include(r => r.GrupoMuscular)
                .Include(r => r.RutinaPlantillaEjercicios)
                    .ThenInclude(rpe => rpe.Ejercicio)
                .Where(r => r.GrupoMuscularId == grupoMuscularId)
                .OrderBy(r => r.Nombre)
                .AsNoTracking()
                .ToListAsync(ct);
        }

    }
}
