using Microsoft.EntityFrameworkCore;
using Gym.Domain.Entities;
using Gym.Infrastructure.Tenant;

namespace Gym.Infrastructure.Persistence;

public class GymDbContext : DbContext
{
    private readonly ITenantService? _tenantService;

    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
    {
    }

    public GymDbContext(DbContextOptions<GymDbContext> options, ITenantService tenantService) : base(options)
    {
        _tenantService = tenantService;
    }

    // Tenants (gimnasios)
    public DbSet<Domain.Entities.Tenant> Tenants => Set<Domain.Entities.Tenant>();

    // Usuarios y autenticación
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Avatar> Avatares => Set<Avatar>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    // Socios y personal
    public DbSet<Socio> Socios => Set<Socio>();
    public DbSet<Personal> Personal => Set<Personal>();

    // Planes y suscripciones
    public DbSet<Plan> Planes => Set<Plan>();
    public DbSet<Suscripcion> Suscripciones => Set<Suscripcion>();

    // Pagos
    public DbSet<OrdenPago> OrdenesPago => Set<OrdenPago>();
    public DbSet<EstadoOrdenPago> EstadosOrdenPago => Set<EstadoOrdenPago>();
    public DbSet<Comprobante> Comprobantes => Set<Comprobante>();

    // Turnos y check-in
    public DbSet<TurnoPlantilla> TurnosPlantilla => Set<TurnoPlantilla>();
    public DbSet<DiaSemana> DiasSemana => Set<DiaSemana>();
    public DbSet<Sala> Salas => Set<Sala>();
    public DbSet<SuscripcionTurno> SuscripcionTurnos => Set<SuscripcionTurno>();
    public DbSet<Checkin> Checkins => Set<Checkin>();

    // Rutinas y ejercicios
    public DbSet<GrupoMuscular> GruposMusculares => Set<GrupoMuscular>();
    public DbSet<Ejercicio> Ejercicios => Set<Ejercicio>();
    public DbSet<RutinaPlantilla> RutinasPlantilla => Set<RutinaPlantilla>();
    public DbSet<RutinaPlantillaEjercicio> RutinaPlantillaEjercicios => Set<RutinaPlantillaEjercicio>();

    // Evolución física
    public DbSet<EvolucionFisica> EvolucionesFisicas => Set<EvolucionFisica>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de nombres de tablas (snake_case para PostgreSQL)
        modelBuilder.Entity<Domain.Entities.Tenant>().ToTable("tenants");
        modelBuilder.Entity<Usuario>().ToTable("usuarios");
        modelBuilder.Entity<Rol>().ToTable("roles");
        modelBuilder.Entity<Avatar>().ToTable("avatares");
        modelBuilder.Entity<PasswordResetToken>().ToTable("password_reset_tokens");
        modelBuilder.Entity<Socio>().ToTable("socios");
        modelBuilder.Entity<Personal>().ToTable("personal");
        modelBuilder.Entity<Plan>().ToTable("planes");
        modelBuilder.Entity<Suscripcion>().ToTable("suscripciones");
        modelBuilder.Entity<OrdenPago>().ToTable("ordenes_pago");
        modelBuilder.Entity<EstadoOrdenPago>().ToTable("estados_orden_pago");
        modelBuilder.Entity<Comprobante>().ToTable("comprobantes");
        modelBuilder.Entity<TurnoPlantilla>().ToTable("turnos_plantilla");
        modelBuilder.Entity<DiaSemana>().ToTable("dias_semana");
        modelBuilder.Entity<Sala>().ToTable("salas");
        modelBuilder.Entity<SuscripcionTurno>().ToTable("suscripcion_turnos");
        modelBuilder.Entity<Checkin>().ToTable("checkins");
        modelBuilder.Entity<GrupoMuscular>().ToTable("grupos_musculares");
        modelBuilder.Entity<Ejercicio>().ToTable("ejercicios");
        modelBuilder.Entity<RutinaPlantilla>().ToTable("rutinas_plantilla");
        modelBuilder.Entity<RutinaPlantillaEjercicio>().ToTable("rutina_plantilla_ejercicios");
        modelBuilder.Entity<EvolucionFisica>().ToTable("evoluciones_fisicas");

        // Global query filter para multi-tenancy
        var tenantId = _tenantService?.GetCurrentTenantId();

        if (tenantId.HasValue)
        {
            modelBuilder.Entity<Usuario>().HasQueryFilter(e => e.TenantId == tenantId.Value);
            modelBuilder.Entity<Socio>().HasQueryFilter(e => e.TenantId == tenantId.Value);
            modelBuilder.Entity<Personal>().HasQueryFilter(e => e.TenantId == tenantId.Value);
            modelBuilder.Entity<Plan>().HasQueryFilter(e => e.TenantId == tenantId.Value);
            modelBuilder.Entity<Suscripcion>().HasQueryFilter(e => e.TenantId == tenantId.Value);
            modelBuilder.Entity<OrdenPago>().HasQueryFilter(e => e.TenantId == tenantId.Value);
            modelBuilder.Entity<Comprobante>().HasQueryFilter(e => e.TenantId == tenantId.Value);
            modelBuilder.Entity<TurnoPlantilla>().HasQueryFilter(e => e.TenantId == tenantId.Value);
            modelBuilder.Entity<Sala>().HasQueryFilter(e => e.TenantId == tenantId.Value);
            modelBuilder.Entity<SuscripcionTurno>().HasQueryFilter(e => e.TenantId == tenantId.Value);
            modelBuilder.Entity<Checkin>().HasQueryFilter(e => e.TenantId == tenantId.Value);
            modelBuilder.Entity<GrupoMuscular>().HasQueryFilter(e => e.TenantId == tenantId.Value);
            modelBuilder.Entity<Ejercicio>().HasQueryFilter(e => e.TenantId == tenantId.Value);
            modelBuilder.Entity<RutinaPlantilla>().HasQueryFilter(e => e.TenantId == tenantId.Value);
            modelBuilder.Entity<RutinaPlantillaEjercicio>().HasQueryFilter(e => e.TenantId == tenantId.Value);
            modelBuilder.Entity<EvolucionFisica>().HasQueryFilter(e => e.TenantId == tenantId.Value);
        }

        // Configuraciones adicionales
        ConfigureRelationships(modelBuilder);
    }

    private void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        // Usuario - Rol
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.RolId);

        // Usuario - Avatar
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Avatar)
            .WithMany(a => a.Usuarios)
            .HasForeignKey(u => u.AvatarId);

        // Usuario - Personal
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Personal)
            .WithOne(p => p.Usuario)
            .HasForeignKey<Usuario>(u => u.PersonalId);

        // Usuario - Socio
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Socio)
            .WithOne(s => s.Usuario)
            .HasForeignKey<Usuario>(u => u.SocioId);

        // PasswordResetToken - Usuario
        modelBuilder.Entity<PasswordResetToken>()
            .HasOne(p => p.Usuario)
            .WithMany()
            .HasForeignKey(p => p.UsuarioId);

        // Socio - Suscripciones
        modelBuilder.Entity<Suscripcion>()
            .HasOne(s => s.Socio)
            .WithMany(so => so.Suscripciones)
            .HasForeignKey(s => s.SocioId);

        // Suscripcion - Plan
        modelBuilder.Entity<Suscripcion>()
            .HasOne(s => s.Plan)
            .WithMany(p => p.Suscripciones)
            .HasForeignKey(s => s.PlanId);

        // OrdenPago - Socio
        modelBuilder.Entity<OrdenPago>()
            .HasOne(o => o.Socio)
            .WithMany(s => s.OrdenesPago)
            .HasForeignKey(o => o.SocioId);

        // OrdenPago - Plan
        modelBuilder.Entity<OrdenPago>()
            .HasOne(o => o.Plan)
            .WithMany(p => p.OrdenesPago)
            .HasForeignKey(o => o.PlanId);

        // OrdenPago - EstadoOrdenPago
        modelBuilder.Entity<OrdenPago>()
            .HasOne(o => o.Estado)
            .WithMany(e => e.OrdenesPago)
            .HasForeignKey(o => o.EstadoId);

        // OrdenPago - Comprobante
        modelBuilder.Entity<OrdenPago>()
            .HasOne(o => o.Comprobante)
            .WithOne(c => c.OrdenPago)
            .HasForeignKey<OrdenPago>(o => o.ComprobanteId);

        // TurnoPlantilla - DiaSemana
        modelBuilder.Entity<TurnoPlantilla>()
            .HasOne(t => t.DiaSemana)
            .WithMany(d => d.TurnosPlantilla)
            .HasForeignKey(t => t.DiaSemanaId);

        // TurnoPlantilla - Personal
        modelBuilder.Entity<TurnoPlantilla>()
            .HasOne(t => t.Personal)
            .WithMany(p => p.TurnosPlantilla)
            .HasForeignKey(t => t.PersonalId);

        // TurnoPlantilla - Sala
        modelBuilder.Entity<TurnoPlantilla>()
            .HasOne(t => t.Sala)
            .WithMany(s => s.TurnosPlantilla)
            .HasForeignKey(t => t.SalaId);

        // SuscripcionTurno - Suscripcion
        modelBuilder.Entity<SuscripcionTurno>()
            .HasOne(st => st.Suscripcion)
            .WithMany(s => s.SuscripcionTurnos)
            .HasForeignKey(st => st.SuscripcionId);

        // SuscripcionTurno - TurnoPlantilla
        modelBuilder.Entity<SuscripcionTurno>()
            .HasOne(st => st.TurnoPlantilla)
            .WithMany(t => t.SuscripcionTurnos)
            .HasForeignKey(st => st.TurnoPlantillaId);

        // SuscripcionTurno - RutinaPlantilla
        modelBuilder.Entity<SuscripcionTurno>()
            .HasOne(st => st.Rutina)
            .WithMany(r => r.SuscripcionTurnos)
            .HasForeignKey(st => st.RutinaId);

        // Checkin - Socio
        modelBuilder.Entity<Checkin>()
            .HasOne(c => c.Socio)
            .WithMany(s => s.Checkins)
            .HasForeignKey(c => c.SocioId);

        // Checkin - TurnoPlantilla
        modelBuilder.Entity<Checkin>()
            .HasOne(c => c.TurnoPlantilla)
            .WithMany(t => t.Checkins)
            .HasForeignKey(c => c.TurnoPlantillaId);

        // Checkin - Personal (Profesor)
        modelBuilder.Entity<Checkin>()
            .HasOne(c => c.Profesor)
            .WithMany(p => p.Checkins)
            .HasForeignKey(c => c.ProfesorId);

        // Ejercicio - GrupoMuscular
        modelBuilder.Entity<Ejercicio>()
            .HasOne(e => e.GrupoMuscular)
            .WithMany(g => g.Ejercicios)
            .HasForeignKey(e => e.GrupoMuscularId);

        // RutinaPlantilla - GrupoMuscular
        modelBuilder.Entity<RutinaPlantilla>()
            .HasOne(r => r.GrupoMuscular)
            .WithMany(g => g.RutinasPlantilla)
            .HasForeignKey(r => r.GrupoMuscularId);

        // RutinaPlantillaEjercicio - RutinaPlantilla
        modelBuilder.Entity<RutinaPlantillaEjercicio>()
            .HasOne(rpe => rpe.RutinaPlantilla)
            .WithMany(r => r.RutinaPlantillaEjercicios)
            .HasForeignKey(rpe => rpe.RutinaId);

        // RutinaPlantillaEjercicio - Ejercicio
        modelBuilder.Entity<RutinaPlantillaEjercicio>()
            .HasOne(rpe => rpe.Ejercicio)
            .WithMany(e => e.RutinaPlantillaEjercicios)
            .HasForeignKey(rpe => rpe.EjercicioId);

        // EvolucionFisica - Socio
        modelBuilder.Entity<EvolucionFisica>()
            .HasOne(ef => ef.Socio)
            .WithMany(s => s.EvolucionesFisicas)
            .HasForeignKey(ef => ef.SocioId);

        // Decimal precision para PostgreSQL
        modelBuilder.Entity<Plan>()
            .Property(p => p.Precio)
            .HasPrecision(10, 2);

        modelBuilder.Entity<OrdenPago>()
            .Property(o => o.Monto)
            .HasPrecision(10, 2);

        modelBuilder.Entity<EvolucionFisica>()
            .Property(e => e.Peso).HasPrecision(5, 2);
        modelBuilder.Entity<EvolucionFisica>()
            .Property(e => e.Altura).HasPrecision(5, 2);
        modelBuilder.Entity<EvolucionFisica>()
            .Property(e => e.Imc).HasPrecision(5, 2);
        modelBuilder.Entity<EvolucionFisica>()
            .Property(e => e.PesoIdeal).HasPrecision(5, 2);
        modelBuilder.Entity<EvolucionFisica>()
            .Property(e => e.Pecho).HasPrecision(5, 2);
        modelBuilder.Entity<EvolucionFisica>()
            .Property(e => e.Cintura).HasPrecision(5, 2);
        modelBuilder.Entity<EvolucionFisica>()
            .Property(e => e.Cadera).HasPrecision(5, 2);
        modelBuilder.Entity<EvolucionFisica>()
            .Property(e => e.Brazo).HasPrecision(5, 2);
        modelBuilder.Entity<EvolucionFisica>()
            .Property(e => e.Pierna).HasPrecision(5, 2);
        modelBuilder.Entity<EvolucionFisica>()
            .Property(e => e.Gemelo).HasPrecision(5, 2);
    }

    public override int SaveChanges()
    {
        SetTenantId();
        SetTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetTenantId();
        SetTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetTenantId()
    {
        var tenantId = _tenantService?.GetCurrentTenantId();
        if (!tenantId.HasValue) return;

        var entries = ChangeTracker.Entries<TenantEntity>()
            .Where(e => e.State == EntityState.Added);

        foreach (var entry in entries)
        {
            entry.Entity.TenantId = tenantId.Value;
        }
    }

    private void SetTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
