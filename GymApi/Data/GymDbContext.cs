using Microsoft.EntityFrameworkCore;
using GymApi.Data.Models;

namespace GymApi.Data;


public partial class GymDbContext : DbContext
{
    public GymDbContext()
    {
    }

    public GymDbContext(DbContextOptions<GymDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<checkin> checkin { get; set; }

    public virtual DbSet<comprobante> comprobante { get; set; }

    public virtual DbSet<ejercicio> ejercicio { get; set; }

    public virtual DbSet<orden_pago> orden_pago { get; set; }

    public virtual DbSet<orden_turno> orden_turno { get; set; }

    public virtual DbSet<plan> plan { get; set; }

    public virtual DbSet<profesor> profesor { get; set; }

    public virtual DbSet<registro_entrenamiento> registro_entrenamiento { get; set; }

    public virtual DbSet<registro_item> registro_item { get; set; }

    public virtual DbSet<rutina_asignada> rutina_asignada { get; set; }

    public virtual DbSet<rutina_plantilla> rutina_plantilla { get; set; }

    public virtual DbSet<rutina_plantilla_ejercicio> rutina_plantilla_ejercicio { get; set; }

    public virtual DbSet<sala> sala { get; set; }

    public virtual DbSet<socio> socio { get; set; }

    public virtual DbSet<suscripcion> suscripcion { get; set; }

    public virtual DbSet<suscripcion_turno> suscripcion_turno { get; set; }

    public virtual DbSet<turno_plantilla> turno_plantilla { get; set; }

    public virtual DbSet<usuario> usuario { get; set; }

    public virtual DbSet<v_checkin_hoy_ar> v_checkin_hoy_ar { get; set; }

    public virtual DbSet<v_cupo_reservado> v_cupo_reservado { get; set; }

    public virtual DbSet<v_ocupacion_hoy> v_ocupacion_hoy { get; set; }

    public virtual DbSet<v_ordenes_ar> v_ordenes_ar { get; set; }

    public virtual DbSet<v_suscripciones_ar> v_suscripciones_ar { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;port=3306;database=\"qym _oram\";user id=root;treattinyasboolean=true;defaultcommandtimeout=60", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.32-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<checkin>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.turno_plantilla_id, "fk_check_turno");

            entity.HasIndex(e => new { e.socio_id, e.fecha_hora }, "idx_check_socio_fecha");

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.fecha_hora).HasColumnType("datetime");
            entity.Property(e => e.origen)
                .HasDefaultValueSql("'recepcion'")
                .HasColumnType("enum('recepcion','app','dispositivo')");
            entity.Property(e => e.socio_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.turno_plantilla_id).HasColumnType("int(10) unsigned");

            entity.HasOne(d => d.socio).WithMany(p => p.checkin)
                .HasForeignKey(d => d.socio_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_check_socio");

            entity.HasOne(d => d.turno_plantilla).WithMany(p => p.checkin)
                .HasForeignKey(d => d.turno_plantilla_id)
                .HasConstraintName("fk_check_turno");
        });

        modelBuilder.Entity<comprobante>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.orden_id, "idx_comp_orden");

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.file_url).HasColumnType("text");
            entity.Property(e => e.mime_type).HasMaxLength(100);
            entity.Property(e => e.orden_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.subido_en)
                .HasDefaultValueSql("utc_timestamp()")
                .HasColumnType("datetime");

            entity.HasOne(d => d.orden).WithMany(p => p.comprobante)
                .HasForeignKey(d => d.orden_id)
                .HasConstraintName("fk_comp_orden");
        });

        modelBuilder.Entity<ejercicio>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.grupo).HasMaxLength(80);
            entity.Property(e => e.media_url).HasColumnType("text");
            entity.Property(e => e.nombre).HasMaxLength(120);
            entity.Property(e => e.tips).HasColumnType("text");
        });

        modelBuilder.Entity<orden_pago>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.plan_id, "fk_orden_plan");

            entity.HasIndex(e => e.estado, "idx_orden_estado");

            entity.HasIndex(e => e.socio_id, "idx_orden_socio");

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.creado_en)
                .HasDefaultValueSql("utc_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'pendiente'")
                .HasColumnType("enum('pendiente','en_revision','verificado','rechazado','expirado')");
            entity.Property(e => e.monto).HasPrecision(12, 2);
            entity.Property(e => e.notas).HasColumnType("text");
            entity.Property(e => e.plan_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.socio_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.vence_en).HasColumnType("datetime");

            entity.HasOne(d => d.plan).WithMany(p => p.orden_pago)
                .HasForeignKey(d => d.plan_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orden_plan");

            entity.HasOne(d => d.socio).WithMany(p => p.orden_pago)
                .HasForeignKey(d => d.socio_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orden_socio");
        });

        modelBuilder.Entity<orden_turno>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.turno_plantilla_id, "fk_ot_turno");

            entity.HasIndex(e => new { e.orden_id, e.turno_plantilla_id }, "uq_orden_turno").IsUnique();

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.orden_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.turno_plantilla_id).HasColumnType("int(10) unsigned");

            entity.HasOne(d => d.orden).WithMany(p => p.orden_turno)
                .HasForeignKey(d => d.orden_id)
                .HasConstraintName("fk_ot_orden");

            entity.HasOne(d => d.turno_plantilla).WithMany(p => p.orden_turno)
                .HasForeignKey(d => d.turno_plantilla_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ot_turno");
        });

        modelBuilder.Entity<plan>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.nombre, "nombre").IsUnique();

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.activo)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.dias_por_semana).HasColumnType("int(11)");
            entity.Property(e => e.nombre).HasMaxLength(80);
            entity.Property(e => e.precio).HasPrecision(12, 2);
        });

        modelBuilder.Entity<profesor>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.email, "email").IsUnique();

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.email).HasMaxLength(320);
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')");
            entity.Property(e => e.nombre).HasMaxLength(160);
        });

        modelBuilder.Entity<registro_entrenamiento>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.rutina_asignada_id, "fk_reg_ruta");

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.observaciones).HasColumnType("text");
            entity.Property(e => e.rutina_asignada_id).HasColumnType("int(10) unsigned");

            entity.HasOne(d => d.rutina_asignada).WithMany(p => p.registro_entrenamiento)
                .HasForeignKey(d => d.rutina_asignada_id)
                .HasConstraintName("fk_reg_ruta");
        });

        modelBuilder.Entity<registro_item>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.ejercicio_id, "fk_rit_ej");

            entity.HasIndex(e => e.registro_id, "fk_rit_reg");

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.carga).HasPrecision(8, 2);
            entity.Property(e => e.ejercicio_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.registro_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.repeticiones).HasColumnType("int(11)");
            entity.Property(e => e.series).HasColumnType("int(11)");

            entity.HasOne(d => d.ejercicio).WithMany(p => p.registro_item)
                .HasForeignKey(d => d.ejercicio_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_rit_ej");

            entity.HasOne(d => d.registro).WithMany(p => p.registro_item)
                .HasForeignKey(d => d.registro_id)
                .HasConstraintName("fk_rit_reg");
        });

        modelBuilder.Entity<rutina_asignada>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.rutina_id, "fk_ruta_plant");

            entity.HasIndex(e => e.socio_id, "fk_ruta_socio");

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.notas).HasColumnType("text");
            entity.Property(e => e.rutina_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.socio_id).HasColumnType("int(10) unsigned");

            entity.HasOne(d => d.rutina).WithMany(p => p.rutina_asignada)
                .HasForeignKey(d => d.rutina_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ruta_plant");

            entity.HasOne(d => d.socio).WithMany(p => p.rutina_asignada)
                .HasForeignKey(d => d.socio_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ruta_socio");
        });

        modelBuilder.Entity<rutina_plantilla>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.plan_id, "fk_rutpla_plan");

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.nombre).HasMaxLength(120);
            entity.Property(e => e.objetivo).HasMaxLength(120);
            entity.Property(e => e.plan_id).HasColumnType("int(10) unsigned");

            entity.HasOne(d => d.plan).WithMany(p => p.rutina_plantilla)
                .HasForeignKey(d => d.plan_id)
                .HasConstraintName("fk_rutpla_plan");
        });

        modelBuilder.Entity<rutina_plantilla_ejercicio>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.ejercicio_id, "fk_rpe_ejercicio");

            entity.HasIndex(e => new { e.rutina_id, e.ejercicio_id }, "uq_rutpla_ej").IsUnique();

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.descanso_seg).HasColumnType("int(11)");
            entity.Property(e => e.ejercicio_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.orden)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)");
            entity.Property(e => e.repeticiones).HasColumnType("int(11)");
            entity.Property(e => e.rutina_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.series).HasColumnType("int(11)");

            entity.HasOne(d => d.ejercicio).WithMany(p => p.rutina_plantilla_ejercicio)
                .HasForeignKey(d => d.ejercicio_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_rpe_ejercicio");

            entity.HasOne(d => d.rutina).WithMany(p => p.rutina_plantilla_ejercicio)
                .HasForeignKey(d => d.rutina_id)
                .HasConstraintName("fk_rpe_rutina");
        });

        modelBuilder.Entity<sala>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.nombre, "nombre").IsUnique();

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.activa)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.capacidad).HasColumnType("int(11)");
            entity.Property(e => e.nombre).HasMaxLength(80);
        });

        modelBuilder.Entity<socio>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.dni, "dni").IsUnique();

            entity.HasIndex(e => e.email, "email").IsUnique();

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.activo)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.creado_en)
                .HasDefaultValueSql("utc_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.dni).HasMaxLength(32);
            entity.Property(e => e.email).HasMaxLength(320);
            entity.Property(e => e.nombre).HasMaxLength(160);
            entity.Property(e => e.telefono).HasMaxLength(64);
        });

        modelBuilder.Entity<suscripcion>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.plan_id, "fk_sus_plan");

            entity.HasIndex(e => e.socio_id, "fk_sus_socio");

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.creado_en)
                .HasDefaultValueSql("utc_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'activa'")
                .HasColumnType("enum('activa','vencida','cancelada')");
            entity.Property(e => e.fin).HasColumnType("datetime");
            entity.Property(e => e.inicio).HasColumnType("datetime");
            entity.Property(e => e.plan_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.socio_id).HasColumnType("int(10) unsigned");

            entity.HasOne(d => d.plan).WithMany(p => p.suscripcion)
                .HasForeignKey(d => d.plan_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sus_plan");

            entity.HasOne(d => d.socio).WithMany(p => p.suscripcion)
                .HasForeignKey(d => d.socio_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sus_socio");
        });

        modelBuilder.Entity<suscripcion_turno>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.turno_plantilla_id, "fk_sust_turno");

            entity.HasIndex(e => new { e.suscripcion_id, e.turno_plantilla_id }, "uq_sus_turno").IsUnique();

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.suscripcion_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.turno_plantilla_id).HasColumnType("int(10) unsigned");

            entity.HasOne(d => d.suscripcion).WithMany(p => p.suscripcion_turno)
                .HasForeignKey(d => d.suscripcion_id)
                .HasConstraintName("fk_sust_sus");

            entity.HasOne(d => d.turno_plantilla).WithMany(p => p.suscripcion_turno)
                .HasForeignKey(d => d.turno_plantilla_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sust_turno");
        });

        modelBuilder.Entity<turno_plantilla>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.profesor_id, "fk_turno_profesor");

            entity.HasIndex(e => e.sala_id, "fk_turno_sala");

            entity.HasIndex(e => new { e.dia_semana, e.hora_inicio, e.sala_id }, "uq_turno").IsUnique();

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.activo)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.cupo).HasColumnType("int(11)");
            entity.Property(e => e.dia_semana).HasColumnType("tinyint(4)");
            entity.Property(e => e.duracion_min).HasColumnType("int(11)");
            entity.Property(e => e.hora_inicio).HasColumnType("time");
            entity.Property(e => e.profesor_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.sala_id).HasColumnType("int(10) unsigned");

            entity.HasOne(d => d.profesor).WithMany(p => p.turno_plantilla)
                .HasForeignKey(d => d.profesor_id)
                .HasConstraintName("fk_turno_profesor");

            entity.HasOne(d => d.sala).WithMany(p => p.turno_plantilla)
                .HasForeignKey(d => d.sala_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_turno_sala");
        });

        modelBuilder.Entity<usuario>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.email, "email").IsUnique();

            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.creado_en)
                .HasDefaultValueSql("utc_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.email).HasMaxLength(320);
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'activo'")
                .HasColumnType("enum('activo','inactivo')");
            entity.Property(e => e.password_hash).HasColumnType("text");
            entity.Property(e => e.rol).HasColumnType("enum('admin','recepcion','profesor')");
        });

        modelBuilder.Entity<v_checkin_hoy_ar>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_checkin_hoy_ar");

            entity.Property(e => e.fecha_ar).HasColumnType("datetime");
            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.origen)
                .HasDefaultValueSql("'recepcion'")
                .HasColumnType("enum('recepcion','app','dispositivo')");
            entity.Property(e => e.socio_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.turno_plantilla_id).HasColumnType("int(10) unsigned");
        });

        modelBuilder.Entity<v_cupo_reservado>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_cupo_reservado");

            entity.Property(e => e.cupo).HasColumnType("int(11)");
            entity.Property(e => e.dia_semana).HasColumnType("tinyint(4)");
            entity.Property(e => e.hora_inicio).HasColumnType("time");
            entity.Property(e => e.reservados).HasColumnType("bigint(21)");
            entity.Property(e => e.turno_id).HasColumnType("int(10) unsigned");
        });

        modelBuilder.Entity<v_ocupacion_hoy>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_ocupacion_hoy");

            entity.Property(e => e.asistencias).HasColumnType("bigint(21)");
            entity.Property(e => e.turno_id).HasColumnType("int(10) unsigned");
        });

        modelBuilder.Entity<v_ordenes_ar>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_ordenes_ar");

            entity.Property(e => e.creado_en)
                .HasDefaultValueSql("utc_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'pendiente'")
                .HasColumnType("enum('pendiente','en_revision','verificado','rechazado','expirado')");
            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.monto).HasPrecision(12, 2);
            entity.Property(e => e.plan_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.socio_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.vence_en_ar).HasColumnType("datetime");
        });

        modelBuilder.Entity<v_suscripciones_ar>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_suscripciones_ar");

            entity.Property(e => e.creado_en)
                .HasDefaultValueSql("utc_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.estado)
                .HasDefaultValueSql("'activa'")
                .HasColumnType("enum('activa','vencida','cancelada')");
            entity.Property(e => e.fin_ar).HasColumnType("datetime");
            entity.Property(e => e.id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.inicio_ar).HasColumnType("datetime");
            entity.Property(e => e.plan_id).HasColumnType("int(10) unsigned");
            entity.Property(e => e.socio_id).HasColumnType("int(10) unsigned");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
