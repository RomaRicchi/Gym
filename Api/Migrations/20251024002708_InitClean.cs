using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class InitClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "avatar",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    url = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    es_predeterminado = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_avatar", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "comprobante",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    file_url = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mime_type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    subido_en = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comprobante", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "dia_semana",
                columns: table => new
                {
                    id = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    nombre = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dia_semana", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ejercicio",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    grupo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tips = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    media_url = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ejercicio", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "estado_orden_pago",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descripcion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estado_orden_pago", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "personal",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefono = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    direccion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    especialidad = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estado = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personal", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "plan",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dias_por_semana = table.Column<int>(type: "int", nullable: false),
                    precio = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rol",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rol", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sala",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cupo = table.Column<int>(type: "int", nullable: false),
                    activa = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sala", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "socio",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    dni = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_nacimiento = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    telefono = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    creado_en = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_socio", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "v_checkin_hoy_ar",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    socio_id = table.Column<int>(type: "int", nullable: false),
                    turno_plantilla_id = table.Column<int>(type: "int", nullable: true),
                    fecha_ar = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    origen = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "v_cupo_reservado",
                columns: table => new
                {
                    turno_id = table.Column<int>(type: "int", nullable: false),
                    dia_semana = table.Column<byte>(type: "tinyint", nullable: false),
                    hora_inicio = table.Column<TimeOnly>(type: "time(6)", nullable: false),
                    cupo = table.Column<int>(type: "int", nullable: false),
                    reservados = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "v_ocupacion_hoy",
                columns: table => new
                {
                    turno_id = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: true),
                    asistencias = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "v_ordenes_ar",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    socio_id = table.Column<int>(type: "int", nullable: false),
                    plan_id = table.Column<int>(type: "int", nullable: false),
                    monto = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    estado = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    vence_en_ar = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    creado_en = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "v_suscripciones_ar",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    socio_id = table.Column<int>(type: "int", nullable: false),
                    plan_id = table.Column<int>(type: "int", nullable: false),
                    inicio_ar = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    fin_ar = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    estado = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    creado_en = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rutina_plantilla",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    objetivo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    plan_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rutina_plantilla", x => x.id);
                    table.ForeignKey(
                        name: "FK_rutina_plantilla_plan_plan_id",
                        column: x => x.plan_id,
                        principalTable: "plan",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "turno_plantilla",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sala_id = table.Column<int>(type: "int", nullable: false),
                    personal_id = table.Column<int>(type: "int", nullable: false),
                    dia_semana_id = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    hora_inicio = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    duracion_min = table.Column<int>(type: "int", nullable: false),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                   
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_turno_plantilla", x => x.id);
                    table.ForeignKey(
                        name: "FK_turno_plantilla_dia_semana_dia_semana_id",
                        column: x => x.dia_semana_id,
                        principalTable: "dia_semana",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    
                    table.ForeignKey(
                        name: "FK_turno_plantilla_personal_personal_id",
                        column: x => x.personal_id,
                        principalTable: "personal",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                  
                    table.ForeignKey(
                        name: "FK_turno_plantilla_sala_sala_id",
                        column: x => x.sala_id,
                        principalTable: "sala",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "orden_pago",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    socio_id = table.Column<int>(type: "int", nullable: false),
                    plan_id = table.Column<int>(type: "int", nullable: false),
                    estado_id = table.Column<int>(type: "int", nullable: false),
                    monto = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    creado_en = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    vence_en = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    notas = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    comprobante_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orden_pago", x => x.id);
                    table.ForeignKey(
                        name: "FK_orden_pago_comprobante_comprobante_id",
                        column: x => x.comprobante_id,
                        principalTable: "comprobante",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_orden_pago_estado_orden_pago_estado_id",
                        column: x => x.estado_id,
                        principalTable: "estado_orden_pago",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orden_pago_plan_plan_id",
                        column: x => x.plan_id,
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orden_pago_socio_socio_id",
                        column: x => x.socio_id,
                        principalTable: "socio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alias = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rol_id = table.Column<int>(type: "int", nullable: false),
                    personal_id = table.Column<int>(type: "int", nullable: true),
                    socio_id = table.Column<int>(type: "int", nullable: true),
                    password_hash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estado = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    creado_en = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    id_avatar = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.id);
                    table.ForeignKey(
                        name: "FK_Usuario_Avatar",
                        column: x => x.id_avatar,
                        principalTable: "avatar",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_usuario_personal_personal_id",
                        column: x => x.personal_id,
                        principalTable: "personal",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_usuario_rol_rol_id",
                        column: x => x.rol_id,
                        principalTable: "rol",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuario_socio_socio_id",
                        column: x => x.socio_id,
                        principalTable: "socio",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rutina_asignada",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    socio_id = table.Column<int>(type: "int", nullable: false),
                    rutina_id = table.Column<int>(type: "int", nullable: false),
                    inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fin = table.Column<DateOnly>(type: "date", nullable: true),
                    notas = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rutina_asignada", x => x.id);
                    table.ForeignKey(
                        name: "FK_rutina_asignada_rutina_plantilla_rutina_id",
                        column: x => x.rutina_id,
                        principalTable: "rutina_plantilla",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rutina_asignada_socio_socio_id",
                        column: x => x.socio_id,
                        principalTable: "socio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rutina_plantilla_ejercicio",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    rutina_id = table.Column<int>(type: "int", nullable: false),
                    ejercicio_id = table.Column<int>(type: "int", nullable: false),
                    orden = table.Column<int>(type: "int", nullable: false),
                    series = table.Column<int>(type: "int", nullable: true),
                    repeticiones = table.Column<int>(type: "int", nullable: true),
                    descanso_seg = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rutina_plantilla_ejercicio", x => x.id);
                    table.ForeignKey(
                        name: "FK_rutina_plantilla_ejercicio_ejercicio_ejercicio_id",
                        column: x => x.ejercicio_id,
                        principalTable: "ejercicio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rutina_plantilla_ejercicio_rutina_plantilla_rutina_id",
                        column: x => x.rutina_id,
                        principalTable: "rutina_plantilla",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "checkin",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    socio_id = table.Column<int>(type: "int", nullable: false),
                    turno_plantilla_id = table.Column<int>(type: "int", nullable: false),
                    fecha_hora = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    origen = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkin", x => x.id);
                    table.ForeignKey(
                        name: "FK_checkin_socio_socio_id",
                        column: x => x.socio_id,
                        principalTable: "socio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_checkin_turno_plantilla_turno_plantilla_id",
                        column: x => x.turno_plantilla_id,
                        principalTable: "turno_plantilla",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "suscripcion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    socio_id = table.Column<int>(type: "int", nullable: false),
                    plan_id = table.Column<int>(type: "int", nullable: false),
                    inicio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    fin = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    estado = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    creado_en = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    orden_pago_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suscripcion", x => x.id);
                    table.ForeignKey(
                        name: "FK_suscripcion_orden_pago_orden_pago_id",
                        column: x => x.orden_pago_id,
                        principalTable: "orden_pago",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_suscripcion_plan_plan_id",
                        column: x => x.plan_id,
                        principalTable: "plan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_suscripcion_socio_socio_id",
                        column: x => x.socio_id,
                        principalTable: "socio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_suscripcion_orden_pago_id",
                table: "suscripcion",
                column: "orden_pago_id");

            migrationBuilder.CreateIndex(
                name: "IX_suscripcion_plan_id",
                table: "suscripcion",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_suscripcion_socio_id",
                table: "suscripcion",
                column: "socio_id");


            migrationBuilder.CreateTable(
                name: "password_reset_tokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    usuario_id = table.Column<int>(type: "int", nullable: false),
                    token = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expira = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_password_reset_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_password_reset_tokens_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "registro_entrenamiento",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    rutina_asignada_id = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    observaciones = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registro_entrenamiento", x => x.id);
                    table.ForeignKey(
                        name: "FK_registro_entrenamiento_rutina_asignada_rutina_asignada_id",
                        column: x => x.rutina_asignada_id,
                        principalTable: "rutina_asignada",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "suscripcion_turno",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    suscripcion_id = table.Column<int>(type: "int", nullable: false),
                    turno_plantilla_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suscripcion_turno", x => x.id);
                    table.ForeignKey(
                        name: "FK_suscripcion_turno_suscripcion_suscripcion_id",
                        column: x => x.suscripcion_id,
                        principalTable: "suscripcion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_suscripcion_turno_turno_plantilla_turno_plantilla_id",
                        column: x => x.turno_plantilla_id,
                        principalTable: "turno_plantilla",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "registro_item",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    registro_id = table.Column<int>(type: "int", nullable: false),
                    ejercicio_id = table.Column<int>(type: "int", nullable: true),
                    series = table.Column<int>(type: "int", nullable: true),
                    repeticiones = table.Column<int>(type: "int", nullable: true),
                    carga = table.Column<decimal>(type: "decimal(65,30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registro_item", x => x.id);
                    table.ForeignKey(
                        name: "FK_registro_item_ejercicio_ejercicio_id",
                        column: x => x.ejercicio_id,
                        principalTable: "ejercicio",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_registro_item_registro_entrenamiento_registro_id",
                        column: x => x.registro_id,
                        principalTable: "registro_entrenamiento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_checkin_socio_id",
                table: "checkin",
                column: "socio_id");

            migrationBuilder.CreateIndex(
                name: "IX_checkin_turno_plantilla_id",
                table: "checkin",
                column: "turno_plantilla_id");

            migrationBuilder.CreateIndex(
                name: "IX_orden_pago_comprobante_id",
                table: "orden_pago",
                column: "comprobante_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orden_pago_estado_id",
                table: "orden_pago",
                column: "estado_id");

            migrationBuilder.CreateIndex(
                name: "IX_orden_pago_plan_id",
                table: "orden_pago",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_orden_pago_socio_id",
                table: "orden_pago",
                column: "socio_id");

            migrationBuilder.CreateIndex(
                name: "IX_password_reset_tokens_usuario_id",
                table: "password_reset_tokens",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_registro_entrenamiento_rutina_asignada_id",
                table: "registro_entrenamiento",
                column: "rutina_asignada_id");

            migrationBuilder.CreateIndex(
                name: "IX_registro_item_ejercicio_id",
                table: "registro_item",
                column: "ejercicio_id");

            migrationBuilder.CreateIndex(
                name: "IX_registro_item_registro_id",
                table: "registro_item",
                column: "registro_id");

            migrationBuilder.CreateIndex(
                name: "IX_rutina_asignada_rutina_id",
                table: "rutina_asignada",
                column: "rutina_id");

            migrationBuilder.CreateIndex(
                name: "IX_rutina_asignada_socio_id",
                table: "rutina_asignada",
                column: "socio_id");

            migrationBuilder.CreateIndex(
                name: "IX_rutina_plantilla_plan_id",
                table: "rutina_plantilla",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_rutina_plantilla_ejercicio_ejercicio_id",
                table: "rutina_plantilla_ejercicio",
                column: "ejercicio_id");

            migrationBuilder.CreateIndex(
                name: "IX_rutina_plantilla_ejercicio_rutina_id",
                table: "rutina_plantilla_ejercicio",
                column: "rutina_id");

            migrationBuilder.CreateIndex(
                name: "IX_suscripcion_orden_pago_id",
                table: "suscripcion",
                column: "orden_pago_id");

            migrationBuilder.CreateIndex(
                name: "IX_suscripcion_plan_id",
                table: "suscripcion",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_suscripcion_plan_id",
                table: "suscripcion",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_suscripcion_socio_id",
                table: "suscripcion",
                column: "socio_id");

            migrationBuilder.CreateIndex(
                name: "IX_suscripcion_turno_suscripcion_id",
                table: "suscripcion_turno",
                column: "suscripcion_id");

            migrationBuilder.CreateIndex(
                name: "IX_suscripcion_turno_turno_plantilla_id",
                table: "suscripcion_turno",
                column: "turno_plantilla_id");

            migrationBuilder.CreateIndex(
                name: "IX_turno_plantilla_dia_semana_id",
                table: "turno_plantilla",
                column: "dia_semana_id");


            migrationBuilder.CreateIndex(
                name: "IX_turno_plantilla_personal_id",
                table: "turno_plantilla",
                column: "personal_id");

          
            migrationBuilder.CreateIndex(
                name: "IX_turno_plantilla_sala_id",
                table: "turno_plantilla",
                column: "sala_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_id_avatar",
                table: "usuario",
                column: "id_avatar");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_personal_id",
                table: "usuario",
                column: "personal_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_rol_id",
                table: "usuario",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_socio_id",
                table: "usuario",
                column: "socio_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checkin");

            migrationBuilder.DropTable(
                name: "password_reset_tokens");

            migrationBuilder.DropTable(
                name: "registro_item");

            migrationBuilder.DropTable(
                name: "rutina_plantilla_ejercicio");

            migrationBuilder.DropTable(
                name: "suscripcion_turno");

            migrationBuilder.DropTable(
                name: "v_checkin_hoy_ar");

            migrationBuilder.DropTable(
                name: "v_cupo_reservado");

            migrationBuilder.DropTable(
                name: "v_ocupacion_hoy");

            migrationBuilder.DropTable(
                name: "v_ordenes_ar");

            migrationBuilder.DropTable(
                name: "v_suscripciones_ar");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "registro_entrenamiento");

            migrationBuilder.DropTable(
                name: "ejercicio");

            migrationBuilder.DropTable(
                name: "suscripcion");

            migrationBuilder.DropTable(
                name: "turno_plantilla");

            migrationBuilder.DropTable(
                name: "avatar");

            migrationBuilder.DropTable(
                name: "rol");

            migrationBuilder.DropTable(
                name: "rutina_asignada");

            migrationBuilder.DropTable(
                name: "orden_pago");

            migrationBuilder.DropTable(
                name: "dia_semana");

            migrationBuilder.DropTable(
                name: "personal");

            migrationBuilder.DropTable(
                name: "sala");

            migrationBuilder.DropTable(
                name: "rutina_plantilla");

            migrationBuilder.DropTable(
                name: "comprobante");

            migrationBuilder.DropTable(
                name: "estado_orden_pago");

            migrationBuilder.DropTable(
                name: "socio");

            migrationBuilder.DropTable(
                name: "plan");
        }
    }
}
