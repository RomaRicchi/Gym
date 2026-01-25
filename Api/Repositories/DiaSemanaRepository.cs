using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class DiaSemanaRepository : IDiaSemanaRepository
    {
        private readonly GymDbContext _db;

        public DiaSemanaRepository(GymDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<DiaSemana>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.DiasSemana
                .AsNoTracking()
                .OrderBy(d => d.Id)
                .ToListAsync(ct);
        }

        public async Task<DiaSemana?> GetByIdAsync(byte id, CancellationToken ct = default)
        {
            return await _db.DiasSemana
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, ct);
        }
    }
}
