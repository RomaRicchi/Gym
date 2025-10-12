using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Api.Data.Models;

namespace Api.Data.Seed
{
    public static class SeedData
    {
        public static async Task RunAsync(GymDbContext db)
        {
            await db.Database.EnsureCreatedAsync();

            // === 🏋️‍♂️ Planes base ===
            if (!await db.Planes.AnyAsync())
            {
                db.Planes.AddRange(
                    new Plan { Nombre = "Plan 2 días", DiasPorSemana = 2, Precio = 18000m, Activo = true },
                    new Plan { Nombre = "Plan 3 días", DiasPorSemana = 3, Precio = 22000m, Activo = true },
                    new Plan { Nombre = "Plan 5 días", DiasPorSemana = 5, Precio = 28000m, Activo = true }
                );
                await db.SaveChangesAsync();
            }

            // === 👥 Personal ===
            var personal = await db.Personales.FirstOrDefaultAsync();
            if (personal is null)
            {
                personal = new Personal
                {
                    Nombre = "Lourdes",
                    Telefono = "123456789",
                    Direccion = "Gimnasio Central",
                    Especialidad = "Fuerza y Tonificación",
                    Estado = true // activo
                };
                db.Personales.Add(personal);
                await db.SaveChangesAsync();
            }

            // === 🏠 Salas ===
            var sala1 = await db.Salas.FirstOrDefaultAsync();
            if (sala1 is null)
            {
                sala1 = new Sala
                {
                    Nombre = "Sala Principal",
                    Capacidad = 20,
                    Activa = true
                };
                db.Salas.Add(sala1);
                await db.SaveChangesAsync();
            }

            // === 🕒 Turnos predefinidos ===
            if (!await db.TurnosPlantilla.AnyAsync())
            {
                TurnoPlantilla Turno(sbyte dia, int hora, int minuto, int duracion, int cupo) =>
                    new TurnoPlantilla
                    {
                        DiaSemana = dia, // 1=Lunes ... 7=Domingo
                        HoraInicio = new TimeOnly(hora, minuto),
                        DuracionMin = duracion,
                        SalaId = sala1.Id,
                        PersonalId = personal.Id, // ✅ reemplaza ProfesorId
                        Cupo = cupo,
                        Activo = true
                    };

                db.TurnosPlantilla.AddRange(
                    Turno(1, 19, 0, 60, 20), // Lunes 19:00
                    Turno(2, 19, 0, 60, 20), // Martes 19:00
                    Turno(3, 19, 0, 60, 20), // Miércoles 19:00
                    Turno(4, 19, 0, 60, 20), // Jueves 19:00
                    Turno(5, 19, 0, 60, 20)  // Viernes 19:00
                );

                await db.SaveChangesAsync();
            }
        }
    }
}
