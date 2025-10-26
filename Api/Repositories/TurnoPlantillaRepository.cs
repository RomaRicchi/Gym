using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class TurnoPlantillaRepository : ITurnoPlantillaRepository
    {
        private readonly GymDbContext _db;

        public TurnoPlantillaRepository(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 Obtener todos
        public async Task<IReadOnlyList<TurnoPlantilla>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .AsNoTracking()
                .OrderBy(t => t.DiaSemanaId)
                .ThenBy(t => t.HoraInicio)
                .ToListAsync(ct);
        }

        // 🔹 Obtener activos
        public async Task<IReadOnlyList<TurnoPlantilla>> GetActivosAsync(CancellationToken ct = default)
        {
            return await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .AsNoTracking()
                .Where(t => t.Activo)
                .OrderBy(t => t.DiaSemanaId)
                .ThenBy(t => t.HoraInicio)
                .ToListAsync(ct);
        }

        // 🔹 Obtener por día — corregido (usa byte)
        public async Task<IReadOnlyList<TurnoPlantilla>> GetByDiaSemanaAsync(byte diaSemana, CancellationToken ct = default)
        {
            Console.WriteLine($"📅 [Repo] Buscando turnos por dia_semana_id={diaSemana}");

            // Se realiza el cast a (byte) ya que DiaSemanaId en el modelo es 'byte' (unsigned tinyint)
            return await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .AsNoTracking()
                .Where(t => t.DiaSemanaId == (byte)diaSemana && t.Activo)
                .OrderBy(t => t.HoraInicio)
                .ToListAsync(ct);
        }

        // 🔹 Obtener por personal
        public async Task<IReadOnlyList<TurnoPlantilla>> GetByPersonalAsync(int personalId, CancellationToken ct = default)
        {
            Console.WriteLine($"👤 [Repo] Buscando turnos por personal_id={personalId}");

            return await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.DiaSemana)
                .AsNoTracking()
                .Where(t => t.PersonalId == personalId && t.Activo)
                .OrderBy(t => t.DiaSemanaId)
                .ThenBy(t => t.HoraInicio)
                .ToListAsync(ct);
        }

        // 🔹 Obtener por ID
        public async Task<TurnoPlantilla?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            Console.WriteLine($"🔎 [Repo] Buscando turno_plantilla id={id}");

            return await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.Personal)
                .Include(t => t.DiaSemana)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        // 🔹 Crear — log detallado
        public async Task<TurnoPlantilla> AddAsync(TurnoPlantilla turno, CancellationToken ct = default)
        {
            try
            {
                Console.WriteLine($"➡️ [ADD] Recibido para insertar:");
                Console.WriteLine($"   SalaId={turno.SalaId}, PersonalId={turno.PersonalId}, DiaSemanaId={turno.DiaSemanaId}");
                Console.WriteLine($"   HoraInicio={turno.HoraInicio}, DuracionMin={turno.DuracionMin}, Cupo={turno.Cupo}, Activo={turno.Activo}");

                _db.TurnosPlantilla.Add(turno);
                await _db.SaveChangesAsync(ct);

                Console.WriteLine($"✅ [ADD] Turno guardado correctamente con ID={turno.Id}");
                return turno;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine("❌ [DB ERROR] " + (dbEx.InnerException?.Message ?? dbEx.Message));
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ [GENERAL ERROR] " + ex.Message);
                throw;
            }
        }

        // 🔹 Actualizar
        public async Task<bool> UpdateAsync(TurnoPlantilla updated, CancellationToken ct = default)
        {
            Console.WriteLine($"✏️ [UPDATE] Intentando actualizar ID={updated.Id}");

            var turno = await _db.TurnosPlantilla.FindAsync(new object[] { updated.Id }, ct);
            if (turno == null)
            {
                Console.WriteLine("⚠️ [UPDATE] Turno no encontrado");
                return false;
            }

            turno.SalaId = updated.SalaId;
            turno.PersonalId = updated.PersonalId;
            turno.DiaSemanaId = updated.DiaSemanaId;
            turno.HoraInicio = updated.HoraInicio;
            turno.DuracionMin = updated.DuracionMin;
            turno.Cupo = updated.Cupo;
            turno.Activo = updated.Activo;

            await _db.SaveChangesAsync(ct);
            Console.WriteLine("✅ [UPDATE] Actualizado correctamente");
            return true;
        }

        // 🔹 Eliminar
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            Console.WriteLine($"🗑 [DELETE] Eliminando turno id={id}");

            var turno = await _db.TurnosPlantilla.FindAsync(new object[] { id }, ct);
            if (turno == null)
            {
                Console.WriteLine("⚠️ [DELETE] Turno no encontrado");
                return false;
            }

            _db.TurnosPlantilla.Remove(turno);
            await _db.SaveChangesAsync(ct);
            Console.WriteLine("✅ [DELETE] Turno eliminado correctamente");
            return true;
        }

        // 🔹 Validar solapamientos
        public async Task<bool> ExisteSolapamientoAsync(int salaId, byte diaSemana, TimeSpan horaInicio, int duracionMin, CancellationToken ct = default)
        {
            var horaFin = horaInicio + TimeSpan.FromMinutes(duracionMin);

            return await _db.TurnosPlantilla
                .AsNoTracking()
                .AnyAsync(t =>
                    t.SalaId == salaId &&
                    t.DiaSemanaId == diaSemana &&
                    t.Activo &&
                    (
                        (t.HoraInicio <= horaInicio && (t.HoraInicio + TimeSpan.FromMinutes(t.DuracionMin)) > horaInicio) ||
                        (t.HoraInicio < horaFin && (t.HoraInicio + TimeSpan.FromMinutes(t.DuracionMin)) >= horaFin)
                    ),
                    ct
                );
        }
    }
}
