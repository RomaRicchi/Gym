using Gym.Application.DTOs.Common;
using Gym.Application.DTOs.Rutinas;
using Gym.Application.Interfaces;
using Gym.Domain.Entities;
using Gym.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.API.Controllers;

[Authorize]
[ApiController]
[Route("api")]
public class RutinasController : ControllerBase
{
    private readonly GymDbContext _db;
    private readonly IFileStorageService _storage;

    public RutinasController(GymDbContext db, IFileStorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    // --- Grupos musculares ---
    [HttpGet("grupomuscular")]
    public async Task<IActionResult> GetGrupos(int page = 1, int pageSize = 50, CancellationToken ct = default)
    {
        var query = _db.GruposMusculares.AsNoTracking();
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(g => g.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(g => new GrupoMuscularDto { Id = g.Id, Nombre = g.Nombre })
            .ToListAsync(ct);

        return Ok(new PaginatedResult<GrupoMuscularDto>
        {
            Items = items,
            TotalItems = total,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("grupomuscular/{id:int}")]
    public async Task<IActionResult> GetGrupo(int id, CancellationToken ct)
    {
        var grupo = await _db.GruposMusculares.FindAsync(new object[] { id }, ct);
        if (grupo == null)
            return NotFound(new { message = "Grupo muscular no encontrado." });

        return Ok(new GrupoMuscularDto { Id = grupo.Id, Nombre = grupo.Nombre });
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpPost("grupomuscular")]
    public async Task<IActionResult> CreateGrupo([FromBody] GrupoMuscularCreateRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            return BadRequest(new { message = "El nombre es obligatorio." });

        var grupo = new GrupoMuscular { Nombre = request.Nombre.Trim() };
        _db.GruposMusculares.Add(grupo);
        await _db.SaveChangesAsync(ct);

        return Ok(grupo);
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpPut("grupomuscular/{id:int}")]
    public async Task<IActionResult> UpdateGrupo(int id, [FromBody] GrupoMuscularCreateRequest request, CancellationToken ct)
    {
        var grupo = await _db.GruposMusculares.FindAsync(new object[] { id }, ct);
        if (grupo == null)
            return NotFound(new { message = "Grupo muscular no encontrado." });

        grupo.Nombre = request.Nombre;
        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Grupo muscular actualizado." });
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpDelete("grupomuscular/{id:int}")]
    public async Task<IActionResult> DeleteGrupo(int id, CancellationToken ct)
    {
        var grupo = await _db.GruposMusculares.FindAsync(new object[] { id }, ct);
        if (grupo == null)
            return NotFound(new { message = "Grupo muscular no encontrado." });

        _db.GruposMusculares.Remove(grupo);
        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Grupo muscular eliminado." });
    }

    // --- Ejercicios ---
    [HttpGet("ejercicios")]
    public async Task<IActionResult> GetEjercicios(int page = 1, int pageSize = 50, string? q = null, CancellationToken ct = default)
    {
        var query = _db.Ejercicios.Include(e => e.GrupoMuscular).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(e => e.Nombre.Contains(q) || (e.GrupoMuscular.Nombre != null && e.GrupoMuscular.Nombre.Contains(q)));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(e => e.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new EjercicioDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Tips = e.Tips,
                MediaUrl = e.MediaUrl,
                GrupoMuscularId = e.GrupoMuscularId,
                GrupoMuscularNombre = e.GrupoMuscular.Nombre
            }).ToListAsync(ct);

        return Ok(new PaginatedResult<EjercicioDto>
        {
            Items = items,
            TotalItems = total,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("ejercicios/{id:int}")]
    public async Task<IActionResult> GetEjercicio(int id, CancellationToken ct)
    {
        var ejercicio = await _db.Ejercicios.Include(e => e.GrupoMuscular).FirstOrDefaultAsync(e => e.Id == id, ct);
        if (ejercicio == null)
            return NotFound(new { message = "Ejercicio no encontrado." });

        return Ok(new EjercicioDto
        {
            Id = ejercicio.Id,
            Nombre = ejercicio.Nombre,
            Tips = ejercicio.Tips,
            MediaUrl = ejercicio.MediaUrl,
            GrupoMuscularId = ejercicio.GrupoMuscularId,
            GrupoMuscularNombre = ejercicio.GrupoMuscular?.Nombre
        });
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpPost("ejercicios")]
    public async Task<IActionResult> CreateEjercicio([FromForm] EjercicioCreateRequest request, IFormFile? Imagen, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            return BadRequest(new { message = "El nombre es obligatorio." });

        string? mediaUrl = null;
        if (Imagen != null && Imagen.Length > 0)
        {
            await using var stream = Imagen.OpenReadStream();
            mediaUrl = await _storage.SaveAsync(stream, Imagen.FileName, "ejercicios", ct);
        }

        var ejercicio = new Ejercicio
        {
            Nombre = request.Nombre,
            Tips = request.Tips,
            GrupoMuscularId = request.GrupoMuscularId,
            MediaUrl = mediaUrl
        };

        _db.Ejercicios.Add(ejercicio);
        await _db.SaveChangesAsync(ct);

        return Ok(new { message = "Ejercicio creado correctamente.", id = ejercicio.Id });
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpPut("ejercicios/{id:int}")]
    public async Task<IActionResult> UpdateEjercicio(int id, [FromForm] EjercicioUpdateRequest request, IFormFile? Imagen, CancellationToken ct)
    {
        var ejercicio = await _db.Ejercicios.FindAsync(new object[] { id }, ct);
        if (ejercicio == null)
            return NotFound(new { message = "Ejercicio no encontrado." });

        ejercicio.Nombre = request.Nombre;
        ejercicio.Tips = request.Tips;
        ejercicio.GrupoMuscularId = request.GrupoMuscularId;

        if (Imagen != null && Imagen.Length > 0)
        {
            await using var stream = Imagen.OpenReadStream();
            ejercicio.MediaUrl = await _storage.SaveAsync(stream, Imagen.FileName, "ejercicios", ct);
        }
        else if (!string.IsNullOrWhiteSpace(request.MediaUrl))
        {
            ejercicio.MediaUrl = request.MediaUrl;
        }

        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Ejercicio actualizado correctamente." });
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpDelete("ejercicios/{id:int}")]
    public async Task<IActionResult> DeleteEjercicio(int id, CancellationToken ct)
    {
        var ejercicio = await _db.Ejercicios.FindAsync(new object[] { id }, ct);
        if (ejercicio == null)
            return NotFound(new { message = "Ejercicio no encontrado." });

        _db.Ejercicios.Remove(ejercicio);
        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Ejercicio eliminado correctamente." });
    }

    // --- Rutinas plantilla ---
    [HttpGet("rutinasplantilla")]
    public async Task<IActionResult> GetRutinas(int page = 1, int pageSize = 20, string? q = null, CancellationToken ct = default)
    {
        var query = _db.RutinasPlantilla.Include(r => r.GrupoMuscular).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(r => r.Nombre.Contains(q) || (r.GrupoMuscular.Nombre != null && r.GrupoMuscular.Nombre.Contains(q)));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(r => r.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new RutinaPlantillaDto
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Objetivo = r.Objetivo,
                GrupoMuscularId = r.GrupoMuscularId,
                GrupoMuscularNombre = r.GrupoMuscular.Nombre,
                ImagenUrl = r.ImagenUrl
            }).ToListAsync(ct);

        return Ok(new PaginatedResult<RutinaPlantillaDto>
        {
            Items = items,
            TotalItems = total,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("rutinasplantilla/all")]
    public async Task<IActionResult> GetRutinasAll(CancellationToken ct)
    {
        var rutinas = await _db.RutinasPlantilla.Include(r => r.GrupoMuscular).AsNoTracking().ToListAsync(ct);
        var result = rutinas.Select(r => new RutinaPlantillaDto
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Objetivo = r.Objetivo,
            GrupoMuscularId = r.GrupoMuscularId,
            GrupoMuscularNombre = r.GrupoMuscular?.Nombre,
            ImagenUrl = r.ImagenUrl
        });

        return Ok(result);
    }

    [HttpGet("rutinasplantilla/{id:int}")]
    public async Task<IActionResult> GetRutina(int id, CancellationToken ct)
    {
        var rutina = await _db.RutinasPlantilla
            .Include(r => r.GrupoMuscular)
            .Include(r => r.RutinaPlantillaEjercicios)
                .ThenInclude(e => e.Ejercicio)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        if (rutina == null)
            return NotFound(new { message = "Rutina no encontrada." });

        return Ok(new RutinaPlantillaDetalleDto
        {
            Id = rutina.Id,
            Nombre = rutina.Nombre,
            Objetivo = rutina.Objetivo,
            GrupoMuscularId = rutina.GrupoMuscularId,
            GrupoMuscularNombre = rutina.GrupoMuscular?.Nombre,
            ImagenUrl = rutina.ImagenUrl,
            Ejercicios = rutina.RutinaPlantillaEjercicios
                .OrderBy(e => e.Orden)
                .Select(e => new RutinaPlantillaEjercicioDto
                {
                    Id = e.Id,
                    RutinaId = e.RutinaId,
                    EjercicioId = e.EjercicioId,
                    EjercicioNombre = e.Ejercicio?.Nombre,
                    Orden = e.Orden,
                    Series = e.Series,
                    Repeticiones = e.Repeticiones,
                    DescansoSeg = e.DescansoSeg
                }).ToList()
        });
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpPost("rutinasplantilla")]
    public async Task<IActionResult> CreateRutina([FromForm] RutinaPlantillaCreateRequest request, IFormFile? Imagen, CancellationToken ct)
    {
        string? imagenUrl = null;
        if (Imagen != null && Imagen.Length > 0)
        {
            await using var stream = Imagen.OpenReadStream();
            imagenUrl = await _storage.SaveAsync(stream, Imagen.FileName, "rutinas", ct);
        }

        var rutina = new RutinaPlantilla
        {
            Nombre = request.Nombre,
            Objetivo = request.Objetivo,
            GrupoMuscularId = request.GrupoMuscularId,
            ImagenUrl = imagenUrl
        };

        _db.RutinasPlantilla.Add(rutina);
        await _db.SaveChangesAsync(ct);

        return Ok(new { message = "Rutina creada correctamente.", id = rutina.Id });
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpPut("rutinasplantilla/{id:int}")]
    public async Task<IActionResult> UpdateRutina(int id, [FromForm] RutinaPlantillaUpdateRequest request, IFormFile? Imagen, CancellationToken ct)
    {
        var rutina = await _db.RutinasPlantilla.FindAsync(new object[] { id }, ct);
        if (rutina == null)
            return NotFound(new { message = "Rutina no encontrada." });

        rutina.Nombre = request.Nombre;
        rutina.Objetivo = request.Objetivo;
        rutina.GrupoMuscularId = request.GrupoMuscularId;

        if (Imagen != null && Imagen.Length > 0)
        {
            await using var stream = Imagen.OpenReadStream();
            rutina.ImagenUrl = await _storage.SaveAsync(stream, Imagen.FileName, "rutinas", ct);
        }
        else if (!string.IsNullOrWhiteSpace(request.ImagenUrl))
        {
            rutina.ImagenUrl = request.ImagenUrl;
        }

        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Rutina actualizada correctamente." });
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpDelete("rutinasplantilla/{id:int}")]
    public async Task<IActionResult> DeleteRutina(int id, CancellationToken ct)
    {
        var rutina = await _db.RutinasPlantilla.FindAsync(new object[] { id }, ct);
        if (rutina == null)
            return NotFound(new { message = "Rutina no encontrada." });

        _db.RutinasPlantilla.Remove(rutina);
        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Rutina eliminada correctamente." });
    }

    // --- Ejercicios por rutina ---
    [HttpGet("rutinasplantillaejercicios")]
    public async Task<IActionResult> GetRutinaEjercicios(int page = 1, int pageSize = 50, string? q = null, CancellationToken ct = default)
    {
        var query = _db.RutinaPlantillaEjercicios
            .Include(r => r.RutinaPlantilla)
            .Include(r => r.Ejercicio)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(r => r.RutinaPlantilla.Nombre.Contains(q));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(r => r.RutinaId).ThenBy(r => r.Orden)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new RutinaPlantillaEjercicioDto
            {
                Id = r.Id,
                RutinaId = r.RutinaId,
                RutinaNombre = r.RutinaPlantilla.Nombre,
                EjercicioId = r.EjercicioId,
                EjercicioNombre = r.Ejercicio.Nombre,
                Orden = r.Orden,
                Series = r.Series,
                Repeticiones = r.Repeticiones,
                DescansoSeg = r.DescansoSeg
            }).ToListAsync(ct);

        return Ok(new PaginatedResult<RutinaPlantillaEjercicioDto>
        {
            Items = items,
            TotalItems = total,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("rutinasplantillaejercicios/{id:int}")]
    public async Task<IActionResult> GetRutinaEjercicio(int id, CancellationToken ct)
    {
        var item = await _db.RutinaPlantillaEjercicios
            .Include(r => r.RutinaPlantilla)
            .Include(r => r.Ejercicio)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        if (item == null)
            return NotFound(new { message = "Registro no encontrado." });

        return Ok(new RutinaPlantillaEjercicioDto
        {
            Id = item.Id,
            RutinaId = item.RutinaId,
            RutinaNombre = item.RutinaPlantilla?.Nombre,
            EjercicioId = item.EjercicioId,
            EjercicioNombre = item.Ejercicio?.Nombre,
            Orden = item.Orden,
            Series = item.Series,
            Repeticiones = item.Repeticiones,
            DescansoSeg = item.DescansoSeg
        });
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpPost("rutinasplantillaejercicios")]
    public async Task<IActionResult> CreateRutinaEjercicio([FromBody] RutinaPlantillaEjercicioCreateRequest request, CancellationToken ct)
    {
        var entity = new RutinaPlantillaEjercicio
        {
            RutinaId = request.RutinaId,
            EjercicioId = request.EjercicioId,
            Orden = request.Orden,
            Series = request.Series,
            Repeticiones = request.Repeticiones,
            DescansoSeg = request.DescansoSeg
        };

        _db.RutinaPlantillaEjercicios.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Ok(new { message = "Ejercicio agregado a la rutina.", id = entity.Id });
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpPut("rutinasplantillaejercicios/{id:int}")]
    public async Task<IActionResult> UpdateRutinaEjercicio(int id, [FromBody] RutinaPlantillaEjercicioUpdateRequest request, CancellationToken ct)
    {
        var entity = await _db.RutinaPlantillaEjercicios.FindAsync(new object[] { id }, ct);
        if (entity == null)
            return NotFound(new { message = "Registro no encontrado." });

        entity.Orden = request.Orden;
        entity.Series = request.Series;
        entity.Repeticiones = request.Repeticiones;
        entity.DescansoSeg = request.DescansoSeg;

        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Registro actualizado correctamente." });
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpDelete("rutinasplantillaejercicios/{id:int}")]
    public async Task<IActionResult> DeleteRutinaEjercicio(int id, CancellationToken ct)
    {
        var entity = await _db.RutinaPlantillaEjercicios.FindAsync(new object[] { id }, ct);
        if (entity == null)
            return NotFound(new { message = "Registro no encontrado." });

        _db.RutinaPlantillaEjercicios.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Registro eliminado correctamente." });
    }

    // --- Evolución física ---
    [HttpGet("evolucionfisica/socio/{socioId:int}")]
    public async Task<IActionResult> GetEvolucionSocio(int socioId, CancellationToken ct)
    {
        var registros = await _db.EvolucionesFisicas
            .Where(e => e.SocioId == socioId)
            .OrderByDescending(e => e.Fecha)
            .ToListAsync(ct);

        return Ok(registros.Select(e => new EvolucionFisicaDto
        {
            Id = e.Id,
            SocioId = e.SocioId,
            Fecha = e.Fecha,
            Peso = e.Peso,
            Altura = e.Altura,
            Imc = e.Imc,
            PesoIdeal = e.PesoIdeal,
            Pecho = e.Pecho,
            Cintura = e.Cintura,
            Cadera = e.Cadera,
            Brazo = e.Brazo,
            Pierna = e.Pierna,
            Gemelo = e.Gemelo,
            Observacion = e.Observacion
        }));
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpPost("evolucionfisica")]
    public async Task<IActionResult> CreateEvolucion([FromBody] EvolucionFisicaCreateRequest request, CancellationToken ct)
    {
        var registro = new EvolucionFisica
        {
            SocioId = request.SocioId,
            Fecha = DateTime.UtcNow.Date,
            Peso = request.Peso,
            Altura = request.Altura,
            PesoIdeal = request.PesoIdeal,
            Pecho = request.Pecho,
            Cintura = request.Cintura,
            Cadera = request.Cadera,
            Brazo = request.Brazo,
            Pierna = request.Pierna,
            Gemelo = request.Gemelo,
            Observacion = request.Observacion,
            Imc = request.Altura > 0 ? request.Peso / (request.Altura * request.Altura) : null
        };

        _db.EvolucionesFisicas.Add(registro);
        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Registro creado.", id = registro.Id });
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpPut("evolucionfisica/{id:int}")]
    public async Task<IActionResult> UpdateEvolucion(int id, [FromBody] EvolucionFisicaCreateRequest request, CancellationToken ct)
    {
        var registro = await _db.EvolucionesFisicas.FindAsync(new object[] { id }, ct);
        if (registro == null)
            return NotFound(new { message = "Registro no encontrado." });

        registro.Peso = request.Peso;
        registro.Altura = request.Altura;
        registro.PesoIdeal = request.PesoIdeal;
        registro.Pecho = request.Pecho;
        registro.Cintura = request.Cintura;
        registro.Cadera = request.Cadera;
        registro.Brazo = request.Brazo;
        registro.Pierna = request.Pierna;
        registro.Gemelo = request.Gemelo;
        registro.Observacion = request.Observacion;
        registro.Imc = request.Altura > 0 ? request.Peso / (request.Altura * request.Altura) : null;

        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Registro actualizado." });
    }

    [Authorize(Roles = "Administrador,Recepcion,Profesor")]
    [HttpDelete("evolucionfisica/{id:int}")]
    public async Task<IActionResult> DeleteEvolucion(int id, CancellationToken ct)
    {
        var registro = await _db.EvolucionesFisicas.FindAsync(new object[] { id }, ct);
        if (registro == null)
            return NotFound(new { message = "Registro no encontrado." });

        _db.EvolucionesFisicas.Remove(registro);
        await _db.SaveChangesAsync(ct);
        return Ok(new { message = "Registro eliminado." });
    }
}
