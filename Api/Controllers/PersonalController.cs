using Api.Data;
using Api.Data.Models;
using Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/personal")]
    public class PersonalController : ControllerBase
    {
        private readonly GymDbContext _db;

        public PersonalController(GymDbContext db)
        {
            _db = db;
        }

        // 🔹 GET /api/personal
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct = default)
        {
            var lista = await _db.Personales
                .AsNoTracking()
                .Include(p => p.Usuario)
                .ThenInclude(u => u.Rol)
                .OrderBy(p => p.Nombre)
                .Select(p => new
                {
                    id = p.Id,
                    nombre = p.Nombre,
                    email = p.Usuario != null ? p.Usuario.Email : null,
                    telefono = p.Telefono,
                    direccion = p.Direccion,
                    especialidad = p.Especialidad,
                    rol = p.Usuario != null && p.Usuario.Rol != null
                        ? p.Usuario.Rol.Nombre
                        : "(Sin rol)",
                    activo = p.Estado
                })
                .ToListAsync(ct);

            return Ok(lista);
        }

        // 🔹 GET /api/personal/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
        {
            var p = await _db.Personales
                .AsNoTracking()
                .Include(p => p.Usuario)
                .ThenInclude(u => u.Rol)
                .Where(x => x.Id == id)
                .Select(p => new
                {
                    id = p.Id,
                    nombre = p.Nombre,
                    email = p.Usuario != null ? p.Usuario.Email : null,
                    telefono = p.Telefono,
                    direccion = p.Direccion,
                    especialidad = p.Especialidad,
                    rolId = p.Usuario != null ? (int?)p.Usuario.RolId : null,
                    rolNombre = p.Usuario != null && p.Usuario.Rol != null
                        ? p.Usuario.Rol.Nombre
                        : null,
                    activo = p.Estado
                })
                .FirstOrDefaultAsync(ct);

            if (p is null)
                return NotFound(new { message = "Personal no encontrado." });

            return Ok(p);
        }

        // 🔹 POST /api/personal
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PersonalCreateDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // 🧩 Crear registro de personal
                var nuevo = new Personal
                {
                    Nombre = dto.Nombre,
                    Telefono = dto.Telefono,
                    Direccion = dto.Direccion ?? "-",
                    Especialidad = dto.Especialidad,
                    Estado = dto.Activo
                };

                _db.Personales.Add(nuevo);
                await _db.SaveChangesAsync(ct);

                // 🧩 Crear usuario asociado (solo si tiene email y rol)
                if (!string.IsNullOrEmpty(dto.Email) && dto.RolId.HasValue)
                {
                    var nuevoUsuario = new Usuario
                    {
                        Email = dto.Email,
                        RolId = dto.RolId.Value,
                        PersonalId = nuevo.Id,
                        PasswordHash = "temporal",
                        Estado = dto.Activo,
                        CreadoEn = DateTime.UtcNow
                    };

                    _db.Usuarios.Add(nuevoUsuario);
                    await _db.SaveChangesAsync(ct);
                }

                return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al crear el personal.", error = ex.Message });
            }
        }

        // 🔹 PUT /api/personal/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] PersonalUpdateDto dto, CancellationToken ct = default)
        {
            var p = await _db.Personales
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (p is null)
                return NotFound(new { message = "Personal no encontrado." });

            // 🧩 Actualizar datos básicos
            p.Nombre = dto.Nombre;
            p.Telefono = dto.Telefono;
            p.Especialidad = dto.Especialidad;
            p.Direccion = dto.Direccion ?? p.Direccion;
            p.Estado = dto.Activo;

            // 🧩 Crear usuario si no existe
            if (p.Usuario == null && !string.IsNullOrEmpty(dto.Email) && dto.RolId.HasValue)
            {
                p.Usuario = new Usuario
                {
                    Email = dto.Email,
                    RolId = dto.RolId.Value,
                    PersonalId = p.Id,
                    PasswordHash = "temporal",
                    Estado = dto.Activo,
                    CreadoEn = DateTime.UtcNow
                };
                _db.Usuarios.Add(p.Usuario);
            }
            else if (p.Usuario != null)
            {
                p.Usuario.Email = dto.Email ?? p.Usuario.Email;
                if (dto.RolId.HasValue)
                    p.Usuario.RolId = dto.RolId.Value;
                p.Usuario.Estado = dto.Activo;
            }

            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true, message = "Personal actualizado correctamente." });
        }

        // 🔹 DELETE /api/personal/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct = default)
        {
            var p = await _db.Personales
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (p is null)
                return NotFound(new { message = "Personal no encontrado." });

            p.Estado = false;
            if (p.Usuario != null)
                p.Usuario.Estado = false;

            await _db.SaveChangesAsync(ct);

            return Ok(new { ok = true, message = "Personal desactivado correctamente." });
        }
    }
}
