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

        // 🔹 Obtener todos con cálculo de cupo dinámico
        public async Task<List<SuscripcionTurno>> GetAllAsync(CancellationToken ct = default)
        {
            var lista = await _db.SuscripcionTurnos
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

            foreach (var item in lista)
            {
                var inscriptos = await _db.SuscripcionTurnos
                    .CountAsync(x => x.TurnoPlantillaId == item.TurnoPlantillaId, ct);

                item.TurnoPlantilla.Sala.CupoDisponible =
                    item.TurnoPlantilla.Sala.Cupo - inscriptos;
            }

            return lista;
        }

        // 🔹 Obtener con check-in
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
                        Sala = new
                        {
                            st.TurnoPlantilla.Sala.Nombre,
                            CupoTotal = st.TurnoPlantilla.Sala.Cupo,
                            CupoDisponible = st.TurnoPlantilla.Sala.Cupo -
                                _db.SuscripcionTurnos.Count(x => x.TurnoPlantillaId == st.TurnoPlantillaId)
                        },
                        Personal = new { st.TurnoPlantilla.Personal.Nombre },
                        DiaSemana = new { st.TurnoPlantilla.DiaSemana.Nombre }
                    },
                    CheckinHecho = _db.Checkins.Any(c =>
                        c.SocioId == st.Suscripcion.Socio.Id &&
                        c.TurnoPlantillaId == st.TurnoPlantillaId &&
                        c.FechaHora.Date == DateTime.UtcNow.Date)
                })
                .OrderBy(st => st.Suscripcion.Socio.Nombre)
                .ToListAsync(ct);

            return data.Cast<object>().ToList();
        }

        // 🔹 Obtener por ID
        public async Task<SuscripcionTurno?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.SuscripcionTurnos
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

            if (entity?.TurnoPlantilla?.Sala != null)
            {
                var inscriptos = await _db.SuscripcionTurnos
                    .CountAsync(x => x.TurnoPlantillaId == entity.TurnoPlantillaId, ct);

                entity.TurnoPlantilla.Sala.CupoDisponible =
                    entity.TurnoPlantilla.Sala.Cupo - inscriptos;
            }

            return entity;
        }

        // 🔹 Obtener turnos por suscripción
        public async Task<List<SuscripcionTurno>> GetBySuscripcionAsync(int suscripcionId, CancellationToken ct = default)
        {
            var lista = await _db.SuscripcionTurnos
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

            foreach (var item in lista)
            {
                var inscriptos = await _db.SuscripcionTurnos
                    .CountAsync(x => x.TurnoPlantillaId == item.TurnoPlantillaId, ct);

                item.TurnoPlantilla.Sala.CupoDisponible =
                    item.TurnoPlantilla.Sala.Cupo - inscriptos;
            }

            return lista;
        }

        // 🔹 Obtener turnos por socio
        public async Task<List<SuscripcionTurno>> GetBySocioAsync(int socioId, CancellationToken ct = default)
        {
            var lista = await _db.SuscripcionTurnos
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

            foreach (var item in lista)
            {
                var inscriptos = await _db.SuscripcionTurnos
                    .CountAsync(x => x.TurnoPlantillaId == item.TurnoPlantillaId, ct);

                item.TurnoPlantilla.Sala.CupoDisponible =
                    item.TurnoPlantilla.Sala.Cupo - inscriptos;
            }

            return lista;
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

        // Cancelar turno (libera cupo)
        public async Task<bool> CancelarTurnoAsync(int id, CancellationToken ct = default)
        {       
            var entity = await _db.SuscripcionTurnos
                .Include(st => st.TurnoPlantilla)
                .ThenInclude(tp => tp.Sala)
                .FirstOrDefaultAsync(st => st.Id == id, ct);

            if (entity == null)
                return false;

            // ⚙️ Combinar fecha actual + hora del turno (TimeSpan → DateTime)
            var ahora = DateTime.UtcNow;
            var horaInicio = entity.TurnoPlantilla?.HoraInicio ?? new TimeSpan(0, 0, 0);
            var fechaTurno = ahora.Date.Add(horaInicio);

            if (fechaTurno <= ahora.AddHours(1))
                throw new InvalidOperationException("Demasiado tarde para cancelar el turno (menos de 1 hora).");

            _db.SuscripcionTurnos.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }


        // Reagendar turno dentro de la misma semana
        public async Task<(bool ok, string message)> ReagendarTurnoAsync(
            int suscripcionId, int turnoActualId, int nuevoTurnoId, CancellationToken ct = default)
        {
            var suscripcion = await _db.Suscripciones
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.Id == suscripcionId, ct);

            if (suscripcion == null)
                return (false, "Suscripción no encontrada.");

            // 🔸 Buscar el turno actual
            var actual = await _db.SuscripcionTurnos.FirstOrDefaultAsync(st => st.Id == turnoActualId, ct);
            if (actual == null)
                return (false, "Turno actual no encontrado.");

            // 🔸 Buscar nuevo turno
            var nuevoTurno = await _db.TurnosPlantilla
                .Include(t => t.Sala)
                .Include(t => t.DiaSemana)
                .FirstOrDefaultAsync(t => t.Id == nuevoTurnoId, ct);

            if (nuevoTurno == null)
                return (false, "Nuevo turno no encontrado.");

            // 🔸 Validar que sea dentro de la semana actual
            var hoy = DateTime.UtcNow;
            var finSemana = hoy.Date.AddDays(7 - (int)hoy.DayOfWeek);
            var fechaTurno = hoy.Date.Add(nuevoTurno.HoraInicio);

            if (fechaTurno.Date > finSemana.Date)
                return (false, "Solo se puede reagendar dentro de esta semana.");

            // 🔸 Liberar el turno anterior
            _db.SuscripcionTurnos.Remove(actual);

            // 🔸 Verificar cupo disponible
            var inscriptos = await _db.SuscripcionTurnos
                .CountAsync(st => st.TurnoPlantillaId == nuevoTurnoId, ct);

            var cupo = nuevoTurno.Sala?.Cupo ?? 0;
            if (inscriptos >= cupo)
                return (false, "No hay cupos disponibles para el nuevo turno.");

            // 🔸 Registrar nuevo turno
            var nuevoST = new SuscripcionTurno
            {
                SuscripcionId = suscripcionId,
                TurnoPlantillaId = nuevoTurnoId
            };

            _db.SuscripcionTurnos.Add(nuevoST);
            await _db.SaveChangesAsync(ct);

            return (true, "Turno reagendado correctamente.");
        }


    }
}
