using Api.Data;
using Api.Repositories.Interfaces;
using Api.Repositories;
using Api.Controllers;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// === ⚙️ Configuración básica ===

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

// === 🌐 CORS para el frontend React/Vite ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("dev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// === 🗄️ Configuración de base de datos (MariaDB/MySQL) ===
var cs = builder.Configuration.GetConnectionString("gym_oram");

// Definimos versión explícita para Pomelo (MariaDB 10.4.32)
var serverVersion = new MariaDbServerVersion(new Version(10, 4, 32));

builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseMySql(cs, serverVersion,
        mySqlOptions => mySqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore)));


// === 💾 File storage local para comprobantes o avatares ===
builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();
// Repositories
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<ISocioRepository, SocioRepository>();
builder.Services.AddScoped<IComprobanteRepository, ComprobanteRepository>();
builder.Services.AddScoped<IDiaSemanaRepository, DiaSemanaRepository>();
builder.Services.AddScoped<IEstadoOrdenPagoRepository, EstadoOrdenPagoRepository>();
builder.Services.AddScoped<IOrdenPagoRepository, OrdenPagoRepository>();
builder.Services.AddScoped<ISalaRepository, SalaRepository>();
builder.Services.AddScoped<ITurnoPlantillaRepository, TurnoPlantillaRepository>();
builder.Services.AddScoped<ISuscripcionTurnoRepository, SuscripcionTurnoRepository>();
builder.Services.AddScoped<ISuscripcionRepository, SuscripcionRepository>();
builder.Services.AddScoped<IAvatarRepository, AvatarRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IPerfilRepository, PerfilRepository>();

var app = builder.Build();

// === 🧪 Swagger ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// === 🧩 Middleware global ===
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("dev");
app.UseAuthorization();
app.MapControllers();

// === 🚀 Run ===
app.Run();
