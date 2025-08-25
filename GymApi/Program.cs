using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using GymApi.Data;
using GymApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers + JSON (evita ciclos y oculta nulls)
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// CORS dev (frente Vite en otro puerto)
builder.Services.AddCors(o => o.AddPolicy("dev",
    p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gym API", Version = "v1" });
});

// DbContext (usa tu cadena "GymDb" del appsettings.json)
builder.Services.AddDbContext<GymDbContext>(opt =>
    opt.UseMySql(
        builder.Configuration.GetConnectionString("GymDb"),
        ServerVersion.Create(new Version(10, 4, 32), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb),
        o => o.EnableRetryOnFailure()
    ));

// ⬅️ REGISTROS DE DI: **siempre antes de Build()**
builder.Services.AddScoped<ISocioRepository, SocioRepository>();

// -----------------------------------------------------------
// A PARTIR DE ACÁ, recien construimos la app
// -----------------------------------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Si estás usando HTTP, dejá comentado esto:
// app.UseHttpsRedirection();

app.UseCors("dev");

app.MapControllers();

app.Run();
