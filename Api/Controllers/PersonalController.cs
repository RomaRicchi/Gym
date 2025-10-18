using Api.Data;
using Api.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
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
        public async Task<IActionResult> Crear([FromBody] dynamic dto, CancellationToken ct = default)
        {
            try
            {
                // 🧩 Crear registro de personal
                var nuevo = new Personal
                {
                    Nombre = dto.nombre,
                    Telefono = dto.telefono,
                    Direccion = "-",
                    Especialidad = dto.especialidad,
                    Estado = dto.activo
                };

                _db.Personales.Add(nuevo);
                await _db.SaveChangesAsync(ct);

                // 🧩 Crear usuario asociado con rol
                var nuevoUsuario = new Usuario
                {
                    Email = dto.email,
                    RolId = dto.rol_id,
                    PersonalId = nuevo.Id,
                    PasswordHash = "temporal",
                    Estado = dto.activo,
                    CreadoEn = DateTime.UtcNow
                };
                _db.Usuarios.Add(nuevoUsuario);

                await _db.SaveChangesAsync(ct);

                return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al crear el personal.", error = ex.Message });
            }
        }

        // 🔹 PUT /api/personal/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] dynamic dto, CancellationToken ct = default)
        {
            var p = await _db.Personales
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (p is null)
                return NotFound(new { message = "Personal no encontrado." });

            // 🧩 Actualizar datos básicos
            p.Nombre = dto.nombre;
            p.Telefono = dto.telefono;
            p.Especialidad = dto.especialidad;
            p.Estado = dto.activo;

            // 🧩 Si no tiene usuario → crear uno nuevo
            if (p.Usuario == null)
            {
                p.Usuario = new Usuario
                {
                    Email = dto.email,
                    RolId = dto.rol_id,
                    PersonalId = p.Id,
                    PasswordHash = "temporal",
                    Estado = dto.activo,
                    CreadoEn = DateTime.UtcNow
                };
                _db.Usuarios.Add(p.Usuario);
            }
            else
            {
                p.Usuario.Email = dto.email;
                p.Usuario.RolId = dto.rol_id;
                p.Usuario.Estado = dto.activo;
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
