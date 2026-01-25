using Gym.Domain.Entities;
using Gym.Infrastructure.Persistence;
using Gym.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Gym.API.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(GymDbContext db, IPasswordHasher passwordHasher)
    {
        // Seed Roles
        if (!await db.Roles.AnyAsync())
        {
            db.Roles.AddRange(
                new Rol { Id = 1, Nombre = "Administrador" },
                new Rol { Id = 2, Nombre = "Profesor" },
                new Rol { Id = 3, Nombre = "Recepcion" },
                new Rol { Id = 4, Nombre = "Socio" }
            );
            await db.SaveChangesAsync();
        }

        // Seed Estados de Orden de Pago
        if (!await db.EstadosOrdenPago.AnyAsync())
        {
            db.EstadosOrdenPago.AddRange(
                new EstadoOrdenPago { Id = 1, Nombre = "Pendiente", Descripcion = "Orden pendiente de pago" },
                new EstadoOrdenPago { Id = 2, Nombre = "Aprobada", Descripcion = "Pago aprobado" },
                new EstadoOrdenPago { Id = 3, Nombre = "Rechazada", Descripcion = "Pago rechazado" }
            );
            await db.SaveChangesAsync();
        }

        // Seed Días de la Semana
        if (!await db.DiasSemana.AnyAsync())
        {
            db.DiasSemana.AddRange(
                new DiaSemana { Id = 1, Nombre = "Lunes" },
                new DiaSemana { Id = 2, Nombre = "Martes" },
                new DiaSemana { Id = 3, Nombre = "Miércoles" },
                new DiaSemana { Id = 4, Nombre = "Jueves" },
                new DiaSemana { Id = 5, Nombre = "Viernes" },
                new DiaSemana { Id = 6, Nombre = "Sábado" },
                new DiaSemana { Id = 7, Nombre = "Domingo" }
            );
            await db.SaveChangesAsync();
        }

        // Seed Avatares predeterminados
        if (!await db.Avatares.AnyAsync())
        {
            db.Avatares.AddRange(
                new Avatar { Id = 1, Nombre = "Default", Url = "/images/avatars/default.png", EsPredeterminado = true },
                new Avatar { Id = 2, Nombre = "Avatar 1", Url = "/images/avatars/avatar1.png", EsPredeterminado = false },
                new Avatar { Id = 3, Nombre = "Avatar 2", Url = "/images/avatars/avatar2.png", EsPredeterminado = false }
            );
            await db.SaveChangesAsync();
        }

        // Seed Tenant de demo (primer gimnasio)
        if (!await db.Tenants.AnyAsync())
        {
            var tenant = new Tenant
            {
                Id = 1,
                Nombre = "Gym Demo",
                Slug = "demo",
                Email = "admin@gymdemo.com",
                Activo = true,
                PlanSaas = "pro"
            };
            db.Tenants.Add(tenant);
            await db.SaveChangesAsync();
        }

        // Seed Usuario Admin
        if (!await db.Usuarios.AnyAsync())
        {
            var admin = new Usuario
            {
                Email = "admin@gym.com",
                Alias = "admin",
                PasswordHash = passwordHasher.Hash("admin123"),
                RolId = 1,
                Estado = true,
                AvatarId = 1,
                TenantId = 1
            };
            db.Usuarios.Add(admin);
            await db.SaveChangesAsync();
        }

        // Seed Grupos Musculares
        if (!await db.GruposMusculares.AnyAsync())
        {
            db.GruposMusculares.AddRange(
                new GrupoMuscular { Nombre = "Pecho", TenantId = 1 },
                new GrupoMuscular { Nombre = "Espalda", TenantId = 1 },
                new GrupoMuscular { Nombre = "Hombros", TenantId = 1 },
                new GrupoMuscular { Nombre = "Bíceps", TenantId = 1 },
                new GrupoMuscular { Nombre = "Tríceps", TenantId = 1 },
                new GrupoMuscular { Nombre = "Piernas", TenantId = 1 },
                new GrupoMuscular { Nombre = "Abdominales", TenantId = 1 },
                new GrupoMuscular { Nombre = "Glúteos", TenantId = 1 }
            );
            await db.SaveChangesAsync();
        }

        // Seed Planes básicos
        if (!await db.Planes.AnyAsync())
        {
            db.Planes.AddRange(
                new Plan { Nombre = "Plan Básico", DiasPorSemana = 3, Precio = 5000, Activo = true, TenantId = 1 },
                new Plan { Nombre = "Plan Estándar", DiasPorSemana = 5, Precio = 7500, Activo = true, TenantId = 1 },
                new Plan { Nombre = "Plan Premium", DiasPorSemana = 7, Precio = 10000, Activo = true, TenantId = 1 }
            );
            await db.SaveChangesAsync();
        }

        // Seed Salas
        if (!await db.Salas.AnyAsync())
        {
            db.Salas.AddRange(
                new Sala { Nombre = "Sala Principal", Cupo = 30, Activa = true, TenantId = 1 },
                new Sala { Nombre = "Sala de Spinning", Cupo = 15, Activa = true, TenantId = 1 },
                new Sala { Nombre = "Sala de Yoga", Cupo = 20, Activa = true, TenantId = 1 }
            );
            await db.SaveChangesAsync();
        }
    }
}
