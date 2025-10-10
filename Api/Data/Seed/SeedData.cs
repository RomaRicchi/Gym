using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Api.Data.Models;

namespace Api.Data.Seed;

public static class SeedData
{
    public static async Task RunAsync(GymDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        if (!await db.Plan.AnyAsync())
        {
            db.Plan.AddRange(
                new plan { nombre = "Plan 2 días", dias_por_semana = 2, precio = 18000m },
                new plan { nombre = "Plan 3 días", dias_por_semana = 3, precio = 22000m },
                new plan { nombre = "Plan 5 días", dias_por_semana = 5, precio = 28000m }
            );
            await db.SaveChangesAsync();
        }

        var prof = await db.Profesor.FirstOrDefaultAsync();
        if (prof is null)
        {
            prof = new profesor { nombre = "Lourdes", email = "lourdes@gym.local", estado = "activo" };
            db.Profesor.Add(prof);
            await db.SaveChangesAsync();
        }

        var sala1 = await db.Sala.FirstOrDefaultAsync();
        if (sala1 is null)
        {
            sala1 = new sala { nombre = "Sala Principal", capacidad = 20, activa = true };
            db.Sala.Add(sala1);
            await db.SaveChangesAsync();
        }

        if (!await db.Turno_plantilla.AnyAsync())
        {
            turno_plantilla Turno(sbyte dia, int hora, int minuto, int duracion, int cupo)
                => new turno_plantilla {
                    dia_semana = dia,              // 1=Lunes ... 7=Domingo (ajustá si usás otra convención)
                    hora_inicio = new TimeOnly(hora, minuto),
                    duracion_min = duracion,
                    sala_id = sala1.id,
                    profesor_id = prof.id,
                    cupo = cupo,
                    activo = true
                };

            db.Turno_plantilla.AddRange(
                Turno(1, 19, 0, 60, 20), // Lunes 19:00
                Turno(3, 19, 0, 60, 20), // Miércoles 19:00
                Turno(5, 19, 0, 60, 20), // Viernes 19:00
                Turno(2, 19, 0, 60, 20), // Martes 19:00
                Turno(4, 19, 0, 60, 20)  // Jueves 19:00
            );
            await db.SaveChangesAsync();
        }
    }
}
