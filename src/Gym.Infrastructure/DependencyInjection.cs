using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Gym.Infrastructure.Persistence;
using Gym.Infrastructure.Tenant;
using Gym.Infrastructure.Services;
using Gym.Infrastructure.Security;
using Gym.Infrastructure.HostedServices;
using Gym.Infrastructure.Repositories;
using Gym.Application.Interfaces;
using Gym.Domain.Interfaces;

namespace Gym.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database - PostgreSQL
        services.AddDbContext<GymDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // JWT Authentication
        var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "GymSaasAPI";
        var jwtAudience = configuration["Jwt:Audience"] ?? "GymSaasFrontend";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            };
        });

        services.AddAuthorization();

        // Tenant Service
        services.AddScoped<ITenantService, TenantService>();

        // Repositories
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ISocioRepository, SocioRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<ISuscripcionRepository, SuscripcionRepository>();
        services.AddScoped<IOrdenPagoRepository, OrdenPagoRepository>();

        // Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Background Services
        services.AddHostedService<TurnosSchedulerService>();

        return services;
    }
}
