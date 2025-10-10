using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Repositories.Interfaces;
using Api.Data;

namespace Api.Controllers;

[ApiController]
[Route("api/ping")]
public class PingController : ControllerBase
{
    private readonly GymDbContext _db;
    public PingController(GymDbContext db) => _db = db;

    [HttpGet("socios")]
    public async Task<IActionResult> Socios()
    {
        // Si DB está bien, devuelve el total
        var total = await _db.Socio.CountAsync();
        return Ok(new { ok = true, total });
    }

    [HttpGet("planes")]
    public async Task<IActionResult> Planes()
    {
        var total = await _db.Plan.CountAsync();
        return Ok(new { ok = true, total });
    }
}
