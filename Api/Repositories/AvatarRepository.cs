using Api.Data;
using Api.Data.Models;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class AvatarRepository : IAvatarRepository
    {
        private readonly GymDbContext _db;

        public AvatarRepository(GymDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<Avatar>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Avatares
                .AsNoTracking()
                .OrderBy(a => a.Id)
                .ToListAsync(ct);
        }

        public async Task<Avatar?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Avatares
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, ct);
        }

        public async Task<Avatar> AddAsync(Avatar avatar, CancellationToken ct = default)
        {
            _db.Avatares.Add(avatar);
            await _db.SaveChangesAsync(ct);
            return avatar;
        }

        public async Task<bool> UpdateAsync(Avatar avatar, CancellationToken ct = default)
        {
            var existing = await _db.Avatares.FindAsync(new object[] { avatar.Id }, ct);
            if (existing == null) return false;

            existing.Url = avatar.Url;
            existing.Nombre = avatar.Nombre;
            existing.EsPredeterminado = avatar.EsPredeterminado;

            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var avatar = await _db.Avatares.FindAsync(new object[] { id }, ct);
            if (avatar == null) return false;

            _db.Avatares.Remove(avatar);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
