using Gym.Domain.Entities;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gym.Infrastructure.HostedServices;

public class TurnosSchedulerService : BackgroundService
{
    private readonly ILogger<TurnosSchedulerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _horaEjecucion = new(3, 0, 0); // 03:00 AM UTC

    public TurnosSchedulerService(ILogger<TurnosSchedulerService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Servicio TurnosScheduler iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var ahora = DateTime.UtcNow;
            var proximaEjecucion = ObtenerProximaEjecucion(ahora);

            var delay = proximaEjecucion - ahora;
            _logger.LogInformation("Próxima ejecución programada: {fecha}", proximaEjecucion);

            try
            {
                await Task.Delay(delay, stoppingToken);
                await GenerarTurnosSemanales(stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Ignorar si se cancela por apagado
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en TurnosSchedulerService");
            }
        }
    }

    private DateTime ObtenerProximaEjecucion(DateTime ahora)
    {
        var proxima = ahora.Date.Add(_horaEjecucion);
        if (proxima <= ahora)
            proxima = proxima.AddDays(1);

        return proxima;
    }

    private async Task GenerarTurnosSemanales(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GymDbContext>();

        var hoy = DateTime.UtcNow.Date;
        var lunes = hoy.AddDays(-(int)hoy.DayOfWeek + 1);
        var domingo = lunes.AddDays(6);

        _logger.LogInformation("Generando turnos de la semana {inicio} - {fin}", lunes, domingo);

        // Obtener suscripciones activas (ignorando filtro de tenant para procesar todas)
        var activas = await db.Suscripciones
            .IgnoreQueryFilters()
            .Include(s => s.Plan)
            .Where(s => s.Estado && s.Fin >= hoy)
            .ToListAsync(ct);

        int nuevos = 0;

        foreach (var sus in activas)
        {
            var turnosSuscripcion = await db.SuscripcionTurnos
                .IgnoreQueryFilters()
                .Include(st => st.TurnoPlantilla)
                .Where(st => st.SuscripcionId == sus.Id)
                .ToListAsync(ct);

            foreach (var st in turnosSuscripcion)
            {
                var tp = st.TurnoPlantilla;
                if (tp == null) continue;

                var fecha = lunes.AddDays(tp.DiaSemanaId - 1).Add(tp.HoraInicio);

                bool existe = await db.Checkins
                    .IgnoreQueryFilters()
                    .AnyAsync(c =>
                        c.SocioId == sus.SocioId &&
                        c.TurnoPlantillaId == tp.Id &&
                        c.FechaHora.Date == fecha.Date, ct);

                if (!existe)
                {
                    db.Checkins.Add(new Checkin
                    {
                        SocioId = sus.SocioId,
                        TurnoPlantillaId = tp.Id,
                        FechaHora = fecha,
                        TenantId = sus.TenantId
                    });
                    nuevos++;
                }
            }
        }

        await db.SaveChangesAsync(ct);
        _logger.LogInformation("Check-ins generados: {cantidad}", nuevos);
    }
}
