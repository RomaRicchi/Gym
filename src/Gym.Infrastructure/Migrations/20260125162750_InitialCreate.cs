using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Gym.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "avatares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: true),
                    EsPredeterminado = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_avatares", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "comprobantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileUrl = table.Column<string>(type: "text", nullable: true),
                    MimeType = table.Column<string>(type: "text", nullable: true),
                    SubidoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comprobantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "dias_semana",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dias_semana", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "estados_orden_pago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estados_orden_pago", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "grupos_musculares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grupos_musculares", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "personal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Direccion = table.Column<string>(type: "text", nullable: true),
                    Especialidad = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "planes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    DiasPorSemana = table.Column<int>(type: "integer", nullable: false),
                    Precio = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Cupo = table.Column<int>(type: "integer", nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "socios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Dni = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_socios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    Logo = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Direccion = table.Column<string>(type: "text", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    PlanSaas = table.Column<string>(type: "text", nullable: false),
                    PlanVenceEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ejercicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Tips = table.Column<string>(type: "text", nullable: true),
                    MediaUrl = table.Column<string>(type: "text", nullable: true),
                    GrupoMuscularId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ejercicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ejercicios_grupos_musculares_GrupoMuscularId",
                        column: x => x.GrupoMuscularId,
                        principalTable: "grupos_musculares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rutinas_plantilla",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Objetivo = table.Column<string>(type: "text", nullable: true),
                    GrupoMuscularId = table.Column<int>(type: "integer", nullable: false),
                    ImagenUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rutinas_plantilla", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rutinas_plantilla_grupos_musculares_GrupoMuscularId",
                        column: x => x.GrupoMuscularId,
                        principalTable: "grupos_musculares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "turnos_plantilla",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    DuracionMin = table.Column<int>(type: "integer", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DiaSemanaId = table.Column<int>(type: "integer", nullable: false),
                    PersonalId = table.Column<int>(type: "integer", nullable: false),
                    SalaId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_turnos_plantilla", x => x.Id);
                    table.ForeignKey(
                        name: "FK_turnos_plantilla_dias_semana_DiaSemanaId",
                        column: x => x.DiaSemanaId,
                        principalTable: "dias_semana",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_turnos_plantilla_personal_PersonalId",
                        column: x => x.PersonalId,
                        principalTable: "personal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_turnos_plantilla_salas_SalaId",
                        column: x => x.SalaId,
                        principalTable: "salas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "evoluciones_fisicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SocioId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Peso = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    Altura = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    Imc = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    PesoIdeal = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    Pecho = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    Cintura = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    Cadera = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    Brazo = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    Pierna = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    Gemelo = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    Observacion = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evoluciones_fisicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_evoluciones_fisicas_socios_SocioId",
                        column: x => x.SocioId,
                        principalTable: "socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ordenes_pago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SocioId = table.Column<int>(type: "integer", nullable: false),
                    PlanId = table.Column<int>(type: "integer", nullable: false),
                    EstadoId = table.Column<int>(type: "integer", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    VenceEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    ComprobanteId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordenes_pago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ordenes_pago_comprobantes_ComprobanteId",
                        column: x => x.ComprobanteId,
                        principalTable: "comprobantes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ordenes_pago_estados_orden_pago_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "estados_orden_pago",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ordenes_pago_planes_PlanId",
                        column: x => x.PlanId,
                        principalTable: "planes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ordenes_pago_socios_SocioId",
                        column: x => x.SocioId,
                        principalTable: "socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    RolId = table.Column<int>(type: "integer", nullable: false),
                    PersonalId = table.Column<int>(type: "integer", nullable: true),
                    SocioId = table.Column<int>(type: "integer", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    AvatarId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usuarios_avatares_AvatarId",
                        column: x => x.AvatarId,
                        principalTable: "avatares",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_usuarios_personal_PersonalId",
                        column: x => x.PersonalId,
                        principalTable: "personal",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_usuarios_roles_RolId",
                        column: x => x.RolId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuarios_socios_SocioId",
                        column: x => x.SocioId,
                        principalTable: "socios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "rutina_plantilla_ejercicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RutinaId = table.Column<int>(type: "integer", nullable: false),
                    EjercicioId = table.Column<int>(type: "integer", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Series = table.Column<int>(type: "integer", nullable: false),
                    Repeticiones = table.Column<int>(type: "integer", nullable: false),
                    DescansoSeg = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rutina_plantilla_ejercicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rutina_plantilla_ejercicios_ejercicios_EjercicioId",
                        column: x => x.EjercicioId,
                        principalTable: "ejercicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rutina_plantilla_ejercicios_rutinas_plantilla_RutinaId",
                        column: x => x.RutinaId,
                        principalTable: "rutinas_plantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "checkins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SocioId = table.Column<int>(type: "integer", nullable: false),
                    TurnoPlantillaId = table.Column<int>(type: "integer", nullable: false),
                    ProfesorId = table.Column<int>(type: "integer", nullable: true),
                    FechaHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_checkins_personal_ProfesorId",
                        column: x => x.ProfesorId,
                        principalTable: "personal",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_checkins_socios_SocioId",
                        column: x => x.SocioId,
                        principalTable: "socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_checkins_turnos_plantilla_TurnoPlantillaId",
                        column: x => x.TurnoPlantillaId,
                        principalTable: "turnos_plantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "suscripciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SocioId = table.Column<int>(type: "integer", nullable: false),
                    PlanId = table.Column<int>(type: "integer", nullable: false),
                    Inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    OrdenPagoId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suscripciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_suscripciones_ordenes_pago_OrdenPagoId",
                        column: x => x.OrdenPagoId,
                        principalTable: "ordenes_pago",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_suscripciones_planes_PlanId",
                        column: x => x.PlanId,
                        principalTable: "planes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_suscripciones_socios_SocioId",
                        column: x => x.SocioId,
                        principalTable: "socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "password_reset_tokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Expira = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_password_reset_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_password_reset_tokens_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "suscripcion_turnos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SuscripcionId = table.Column<int>(type: "integer", nullable: false),
                    TurnoPlantillaId = table.Column<int>(type: "integer", nullable: false),
                    RutinaId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suscripcion_turnos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_suscripcion_turnos_rutinas_plantilla_RutinaId",
                        column: x => x.RutinaId,
                        principalTable: "rutinas_plantilla",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_suscripcion_turnos_suscripciones_SuscripcionId",
                        column: x => x.SuscripcionId,
                        principalTable: "suscripciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_suscripcion_turnos_turnos_plantilla_TurnoPlantillaId",
                        column: x => x.TurnoPlantillaId,
                        principalTable: "turnos_plantilla",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_checkins_ProfesorId",
                table: "checkins",
                column: "ProfesorId");

            migrationBuilder.CreateIndex(
                name: "IX_checkins_SocioId",
                table: "checkins",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_checkins_TurnoPlantillaId",
                table: "checkins",
                column: "TurnoPlantillaId");

            migrationBuilder.CreateIndex(
                name: "IX_ejercicios_GrupoMuscularId",
                table: "ejercicios",
                column: "GrupoMuscularId");

            migrationBuilder.CreateIndex(
                name: "IX_evoluciones_fisicas_SocioId",
                table: "evoluciones_fisicas",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_pago_ComprobanteId",
                table: "ordenes_pago",
                column: "ComprobanteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_pago_EstadoId",
                table: "ordenes_pago",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_pago_PlanId",
                table: "ordenes_pago",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ordenes_pago_SocioId",
                table: "ordenes_pago",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_password_reset_tokens_UsuarioId",
                table: "password_reset_tokens",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_rutina_plantilla_ejercicios_EjercicioId",
                table: "rutina_plantilla_ejercicios",
                column: "EjercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_rutina_plantilla_ejercicios_RutinaId",
                table: "rutina_plantilla_ejercicios",
                column: "RutinaId");

            migrationBuilder.CreateIndex(
                name: "IX_rutinas_plantilla_GrupoMuscularId",
                table: "rutinas_plantilla",
                column: "GrupoMuscularId");

            migrationBuilder.CreateIndex(
                name: "IX_suscripcion_turnos_RutinaId",
                table: "suscripcion_turnos",
                column: "RutinaId");

            migrationBuilder.CreateIndex(
                name: "IX_suscripcion_turnos_SuscripcionId",
                table: "suscripcion_turnos",
                column: "SuscripcionId");

            migrationBuilder.CreateIndex(
                name: "IX_suscripcion_turnos_TurnoPlantillaId",
                table: "suscripcion_turnos",
                column: "TurnoPlantillaId");

            migrationBuilder.CreateIndex(
                name: "IX_suscripciones_OrdenPagoId",
                table: "suscripciones",
                column: "OrdenPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_suscripciones_PlanId",
                table: "suscripciones",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_suscripciones_SocioId",
                table: "suscripciones",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_turnos_plantilla_DiaSemanaId",
                table: "turnos_plantilla",
                column: "DiaSemanaId");

            migrationBuilder.CreateIndex(
                name: "IX_turnos_plantilla_PersonalId",
                table: "turnos_plantilla",
                column: "PersonalId");

            migrationBuilder.CreateIndex(
                name: "IX_turnos_plantilla_SalaId",
                table: "turnos_plantilla",
                column: "SalaId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_AvatarId",
                table: "usuarios",
                column: "AvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_PersonalId",
                table: "usuarios",
                column: "PersonalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_RolId",
                table: "usuarios",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_SocioId",
                table: "usuarios",
                column: "SocioId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checkins");

            migrationBuilder.DropTable(
                name: "evoluciones_fisicas");

            migrationBuilder.DropTable(
                name: "password_reset_tokens");

            migrationBuilder.DropTable(
                name: "rutina_plantilla_ejercicios");

            migrationBuilder.DropTable(
                name: "suscripcion_turnos");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "ejercicios");

            migrationBuilder.DropTable(
                name: "rutinas_plantilla");

            migrationBuilder.DropTable(
                name: "suscripciones");

            migrationBuilder.DropTable(
                name: "turnos_plantilla");

            migrationBuilder.DropTable(
                name: "avatares");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "grupos_musculares");

            migrationBuilder.DropTable(
                name: "ordenes_pago");

            migrationBuilder.DropTable(
                name: "dias_semana");

            migrationBuilder.DropTable(
                name: "personal");

            migrationBuilder.DropTable(
                name: "salas");

            migrationBuilder.DropTable(
                name: "comprobantes");

            migrationBuilder.DropTable(
                name: "estados_orden_pago");

            migrationBuilder.DropTable(
                name: "planes");

            migrationBuilder.DropTable(
                name: "socios");
        }
    }
}
