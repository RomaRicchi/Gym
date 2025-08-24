
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymApi.Data; 

[ApiController]
[Route("api/ping")]
public class PingController : ControllerBase
{
    private readonly GymApi.Data.GymDbContext _db;
    public PingController(GymApi.Data.GymDbContext db) => _db = db;

    [HttpGet("planes")]
    public async Task<IActionResult> GetPlanes()
    {
        var planes = await _db.plan.Take(5).ToListAsync();
        return Ok(planes);
    }

    [HttpGet("socios")]
    public async Task<IActionResult> GetSocios()
    {
        var socios = await _db.socio.Take(5).ToListAsync();
        return Ok(socios);
    }
}
