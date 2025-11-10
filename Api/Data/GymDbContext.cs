using Microsoft.EntityFrameworkCore;
using Api.Data.Models;

namespace Api.Data;

public partial class GymDbContext : DbContext
{
    public GymDbContext() { }

    public GymDbContext(DbContextOptions<GymDbContext> options)
        : base(options) { }

    // === Tablas principales ===
    public virtual DbSet<Checkin> Checkins { get; set; }
    public virtual DbSet<Comprobante> Comprobantes { get; set; }
    public virtual DbSet<OrdenPago> OrdenesPago { get; set; } = null!;
    public virtual DbSet<EstadoOrdenPago> EstadoOrdenPago { get; set; } = null!;
    public virtual DbSet<Rol> Roles { get; set; }
    public virtual DbSet<DiaSemana> DiasSemana { get; set; }
    public virtual DbSet<Plan> Planes { get; set; } = null!;
    public virtual DbSet<Personal> Personales { get; set; }
    public virtual DbSet<Sala> Salas { get; set; }
    public virtual DbSet<Socio> Socios { get; set; } = null!;
    public virtual DbSet<Suscripcion> Suscripciones { get; set; }
    public virtual DbSet<SuscripcionTurno> SuscripcionTurnos { get; set; }
    public virtual DbSet<TurnoPlantilla> TurnosPlantilla { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Avatar> Avatares { get; set; } = null!;
    public virtual DbSet<EvolucionFisica> EvolucionFisica { get; set; }
    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public virtual DbSet<Ejercicio> Ejercicios { get; set; } = null!;
    public virtual DbSet<RutinaPlantilla> RutinasPlantilla { get; set; } = null!;
    public virtual DbSet<RutinaPlantillaEjercicio> RutinasPlantillaEjercicios { get; set; } = null!;
    public virtual DbSet<GrupoMuscular> GruposMusculares { get; set; }



    // === Vistas ===
    public virtual DbSet<VOcupacionHoy> VOcupacionHoy { get; set; }
    public virtual DbSet<VOrdenesAr> VOrdenesAr { get; set; }
    public virtual DbSet<VSuscripcionesAr> VSuscripcionesAr { get; set; }
    public virtual DbSet<VCupoReservado> VCupoReservado { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>().ToTable("usuario");
        modelBuilder.Entity<Rol>().ToTable("rol");
        modelBuilder.Entity<Socio>().ToTable("socio");
        modelBuilder.Entity<Personal>().ToTable("personal");
        modelBuilder.Entity<Plan>().ToTable("plan");
        modelBuilder.Entity<Sala>().ToTable("sala");
        modelBuilder.Entity<Checkin>().ToTable("checkin");
        modelBuilder.Entity<Ejercicio>().ToTable("ejercicio");
        modelBuilder.Entity<Comprobante>().ToTable("comprobante");
        modelBuilder.Entity<OrdenPago>().ToTable("orden_pago");
        modelBuilder.Entity<EstadoOrdenPago>().ToTable("estado_orden_pago");
        modelBuilder.Entity<Suscripcion>().ToTable("suscripcion");
        modelBuilder.Entity<SuscripcionTurno>().ToTable("suscripcion_turno");
        modelBuilder.Entity<TurnoPlantilla>().ToTable("turno_plantilla");
        modelBuilder.Entity<RutinaPlantilla>().ToTable("rutina_plantilla");
        modelBuilder.Entity<RutinaPlantillaEjercicio>().ToTable("rutina_plantilla_ejercicio");
        modelBuilder.Entity<DiaSemana>().ToTable("dia_semana");
        modelBuilder.Entity<PasswordResetToken>().ToTable("password_reset_tokens");
        modelBuilder.Entity<EvolucionFisica>().ToTable("evolucion_fisica");
        modelBuilder.Entity<GrupoMuscular>().ToTable("grupo_muscular");
        // Relaciones
        modelBuilder.Entity<EvolucionFisica>()
            .HasOne<Socio>()
            .WithMany()
            .HasForeignKey(e => e.SocioId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_evolucion_socio");
        // Ejercicio -> GrupoMuscular (N:1)
        modelBuilder.Entity<Ejercicio>()
            .HasOne(e => e.GrupoMuscular)
            .WithMany(g => g.Ejercicios)
            .HasForeignKey(e => e.GrupoMuscularId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_ejercicio_grupo_muscular");
        //  RutinaPlantilla -> GrupoMuscular (N:1)
        modelBuilder.Entity<RutinaPlantilla>()
            .HasOne(r => r.GrupoMuscular)
            .WithMany(g => g.RutinasPlantilla)
            .HasForeignKey(r => r.GrupoMuscularId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rutina_grupo_muscular");
        // RutinaPlantillaEjercicio -> RutinaPlantilla (N:1)
        modelBuilder.Entity<RutinaPlantillaEjercicio>()
            .HasOne(rpe => rpe.RutinaPlantilla)
            .WithMany(r => r.RutinaPlantillaEjercicios)
            .HasForeignKey(rpe => rpe.RutinaId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rpe_rutina");
        // RutinaPlantillaEjercicio -> Ejercicio (N:1)
        modelBuilder.Entity<RutinaPlantillaEjercicio>()
            .HasOne(rpe => rpe.Ejercicio)
            .WithMany(e => e.RutinaPlantillaEjercicios)
            .HasForeignKey(rpe => rpe.EjercicioId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rpe_ejercicio");
                        
        modelBuilder.Entity<PasswordResetToken>()
            .HasOne(p => p.Usuario)
            .WithMany()
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Avatar
        modelBuilder.Entity<Avatar>(entity =>
        {
            entity.ToTable("avatar");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Url)
                .HasColumnName("url")
                .HasMaxLength(255)
                .IsRequired();
            entity.Property(e => e.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(100);
            entity.Property(e => e.EsPredeterminado)
                .HasColumnName("es_predeterminado")
                .HasDefaultValue(false);
            entity.HasMany(e => e.Usuarios)
                .WithOne(u => u.Avatar)
                .HasForeignKey(u => u.IdAvatar)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Usuario_Avatar");
        });

        // Vistas sin clave
        modelBuilder.Entity<VOcupacionHoy>().ToTable("v_ocupacion_hoy").HasNoKey();
        modelBuilder.Entity<VOrdenesAr>().ToTable("v_ordenes_ar").HasNoKey();
        modelBuilder.Entity<VSuscripcionesAr>().ToTable("v_suscripciones_ar").HasNoKey();
        modelBuilder.Entity<VCupoReservado>().ToTable("v_cupo_reservado").HasNoKey();

        // Conversión de nombres a snake_case
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (tableName != null)
                entity.SetTableName(ToSnakeCase(tableName));

            if (entity.Name.EndsWith("EvolucionFisica"))
                continue;

            foreach (var property in entity.GetProperties())
                property.SetColumnName(ToSnakeCase(property.Name));
        }
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                    sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}
