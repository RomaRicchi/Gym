using GymApi.Data;
using GymApi.Services;             
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Swagger, Controllers, CORS
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddPolicy("dev", p =>
    p.WithOrigins("http://localhost:5173")
     .AllowAnyHeader()
     .AllowAnyMethod()
));

// MySQL/MariaDB
var cs = builder.Configuration.GetConnectionString("GymDb");
// Si preferís la versión explícita:
var serverVersion = new MariaDbServerVersion(new Version(10, 4, 32));
builder.Services.AddDbContext<GymDbContext>(opt =>
    opt.UseMySql(cs, serverVersion));

// Storage (appsettings: "Storage": { "ComprobantesPath": "..." })
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseCors("dev");
app.UseHttpsRedirection();

app.MapControllers();

// Seed inicial
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GymDbContext>();
    await GymApi.Data.Seed.SeedData.RunAsync(db);
}

app.Run();
