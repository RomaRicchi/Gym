// Program.cs
using Microsoft.EntityFrameworkCore;
using GymApi.Data;
using GymApi.Repositories;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<GymDbContext>(opt =>
    opt.UseMySql(
        builder.Configuration.GetConnectionString("GymDb"),
        ServerVersion.Create(new Version(10,4,32), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb)));

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<ISocioRepository, SocioRepository>();
// CORS (abre para localhost)
builder.Services.AddCors(o => o.AddPolicy("local",
    p => p.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000","http://localhost:5173")));

var app = builder.Build();

app.UseCors("local");

// Swagger UI en dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
