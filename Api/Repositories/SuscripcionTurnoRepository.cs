using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class SuscripcionTurnoRepository : ISuscripcionTurnoRepository
    {
        private readonly GymDbContext _db;

        public SuscripcionTurnoRepository(GymDbContext db)
        {
            _db = db;
        }

        public async Task<List<object>> GetAllWithCheckinAsync(CancellationToken ct = default)
        {
            var data = await _db.SuscripcionTurnos
                .AsNoTracking()
                .Include(st => st.Suscripcion)
                    .ThenInclude(s => s.Socio)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.DiaSemana)
                .Select(st => new
                {
                    st.Id,
                    Suscripcion = new
                    {
                        Socio = new
                        {
                            st.Suscripcion.Socio.Id,
                            st.Suscripcion.Socio.Nombre
                        }
                    },
                    TurnoPlantilla = new
                    {
                        st.TurnoPlantilla.Id,
                        st.TurnoPlantilla.HoraInicio,
                        st.TurnoPlantilla.DuracionMin,
                        st.TurnoPlantilla.Cupo,
                        Sala = new { st.TurnoPlantilla.Sala.Nombre },
                        Personal = new { st.TurnoPlantilla.Personal.Nombre },
                        DiaSemana = new { st.TurnoPlantilla.DiaSemana.Nombre }
                    },
                    // 👇 Calcula si ya hizo check-in hoy
                    CheckinHecho = _db.Checkins.Any(c =>
                        c.SocioId == st.Suscripcion.Socio.Id &&
                        c.TurnoPlantillaId == st.TurnoPlantillaId &&
                        c.FechaHora.Date == DateTime.UtcNow.Date)
                })
                .OrderBy(st => st.Suscripcion.Socio.Nombre)
                .ToListAsync(ct);

            return data.Cast<object>().ToList();
        }

        // 🔹 Obtener todos los turnos asignados (para todos los socios)

        public async Task<List<SuscripcionTurno>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.SuscripcionTurnos
                .AsNoTracking()
                .Include(st => st.Suscripcion)
                    .ThenInclude(s => s.Socio)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.DiaSemana)
                .OrderBy(st => st.Suscripcion.Socio.Nombre)
                .ToListAsync(ct);
        }

        // 🔹 Obtener por ID
        public async Task<SuscripcionTurno?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.SuscripcionTurnos
                .AsNoTracking()
                .Include(st => st.Suscripcion)
                    .ThenInclude(s => s.Socio)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.DiaSemana)
                .FirstOrDefaultAsync(st => st.Id == id, ct);
        }

        // 🔹 Obtener turnos por suscripción
        public async Task<List<SuscripcionTurno>> GetBySuscripcionAsync(int suscripcionId, CancellationToken ct = default)
        {
            return await _db.SuscripcionTurnos
                .Where(st => st.SuscripcionId == suscripcionId)
                .Include(st => st.Suscripcion)
                    .ThenInclude(s => s.Socio)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.DiaSemana)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        // 🔹 Obtener turnos por socio
        public async Task<List<SuscripcionTurno>> GetBySocioAsync(int socioId, CancellationToken ct = default)
        {
            return await _db.SuscripcionTurnos
                .Where(st => st.Suscripcion.SocioId == socioId)
                .Include(st => st.Suscripcion)
                    .ThenInclude(s => s.Plan)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Sala)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.Personal)
                .Include(st => st.TurnoPlantilla)
                    .ThenInclude(tp => tp.DiaSemana)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        // 🔹 Crear
        public async Task<SuscripcionTurno> AddAsync(SuscripcionTurno entity, CancellationToken ct = default)
        {
            _db.SuscripcionTurnos.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        // 🔹 Actualizar
        public async Task UpdateAsync(SuscripcionTurno entity, CancellationToken ct = default)
        {
            _db.SuscripcionTurnos.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        // 🔹 Eliminar
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.SuscripcionTurnos.FindAsync(new object?[] { id }, ct);
            if (entity != null)
            {
                _db.SuscripcionTurnos.Remove(entity);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
