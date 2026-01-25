using Api.Data;
using Api.Repositories.Interfaces;
using Api.Repositories;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// === Configuración básica ===
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// === CORS (para frontend React/Vite en puerto 5173) ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("dev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithExposedHeaders("*");
    });
});

// === 🗄️ Base de datos MySQL/MariaDB ===
var cs = builder.Configuration.GetConnectionString("gym_oram");
var serverVersion = new MariaDbServerVersion(new Version(10, 4, 32));

builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseMySql(cs, serverVersion,
        mySqlOptions => mySqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore)));

// === Autenticación JWT ===
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(key))
            throw new Exception("❌ No se encontró Jwt:Key en appsettings.json");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        };
    });

builder.Services.AddAuthorization();

// === Swagger con soporte para JWT ===
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Gym API", Version = "v1" });

    // Configuración para token JWT
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Ingrese el token JWT en este formato: **Bearer {token}**",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// === Servicios y Repositorios ===
builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();
builder.Services.AddScoped<IEmailService, EmailService>();
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
builder.Services.AddScoped<ICheckinRepository, CheckinRepository>();
builder.Services.AddHostedService<Api.Services.TurnosSchedulerService>();
builder.Services.AddScoped<IEvolucionFisicaRepository, EvolucionFisicaRepository>();
builder.Services.AddScoped<IEjercicioRepository, EjercicioRepository>();
builder.Services.AddScoped<IRutinaPlantillaRepository, RutinaPlantillaRepository>();
builder.Services.AddScoped<IRutinaPlantillaEjercicioRepository, RutinaPlantillaEjercicioRepository>();
builder.Services.AddScoped<IGrupoMuscularRepository, GrupoMuscularRepository>();


var app = builder.Build();

// === Swagger ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// === Middleware global ===
app.UseCors("dev");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
    ),
    RequestPath = "",
    ServeUnknownFileTypes = true,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// === Run ===
app.Run();
