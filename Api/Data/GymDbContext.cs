using Microsoft.EntityFrameworkCore;
using Api.Data.Models;

namespace Api.Data;

public partial class GymDbContext : DbContext
{
    public GymDbContext() { }

    public GymDbContext(DbContextOptions<GymDbContext> options)
        : base(options) { }

    public virtual DbSet<Checkin> Checkins { get; set; }
    public virtual DbSet<Comprobante> Comprobantes { get; set; }
    public virtual DbSet<Ejercicio> Ejercicios { get; set; }
    public virtual DbSet<OrdenPago> OrdenesPago { get; set; }
    public virtual DbSet<OrdenTurno> OrdenesTurno { get; set; }
    public virtual DbSet<Plan> Planes { get; set; }
    public virtual DbSet<Profesor> Profesores { get; set; }
    public virtual DbSet<RegistroEntrenamiento> RegistrosEntrenamiento { get; set; }
    public virtual DbSet<RegistroItem> RegistrosItem { get; set; }
    public virtual DbSet<RutinaAsignada> RutinasAsignadas { get; set; }
    public virtual DbSet<RutinaPlantilla> RutinasPlantilla { get; set; }
    public virtual DbSet<RutinaPlantillaEjercicio> RutinasPlantillaEjercicios { get; set; }
    public virtual DbSet<Sala> Salas { get; set; }
    public virtual DbSet<Socio> Socios { get; set; }
    public virtual DbSet<Suscripcion> Suscripciones { get; set; }
    public virtual DbSet<SuscripcionTurno> SuscripcionesTurno { get; set; }
    public virtual DbSet<TurnoPlantilla> TurnosPlantilla { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<VCheckinHoyAr> VCheckinHoyAr { get; set; }
    public virtual DbSet<VCupoReservado> VCupoReservado { get; set; }
    public virtual DbSet<VOcupacionHoy> VOcupacionHoy { get; set; }
    public virtual DbSet<VOrdenesAr> VOrdenesAr { get; set; }
    public virtual DbSet<VSuscripcionesAr> VSuscripcionesAr { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 🔽 Mapeo automático a snake_case para tablas y columnas
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (tableName != null)
                entity.SetTableName(ToSnakeCase(tableName));

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }
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
