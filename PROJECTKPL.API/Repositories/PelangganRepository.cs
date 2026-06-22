using Microsoft.EntityFrameworkCore;
using PROJECTKPL.API.Data;
using PROJECTKPL.API.Models;

namespace PROJECTKPL.API.Repositories
{
    public class PelangganRepository : IRepository<Pelanggan>
    {
        private readonly AppDbContext _db;

        public PelangganRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Pelanggan>> GetAllAsync()
        {
            return await _db.Pelanggan
                .Select(p => new Pelanggan
                {
                    Id = p.Id,
                    Username = p.Username,
                    Gender = p.Gender,
                    NoTelp = p.NoTelp,
                    Umur = p.Umur,
                    // Password sengaja tidak dikembalikan
                })
                .ToListAsync();
        }

        public async Task<Pelanggan?> GetByIdAsync(int id)
        {
            return await _db.Pelanggan.FindAsync(id);
        }

        public async Task<Pelanggan> AddAsync(Pelanggan pelanggan)
        {
            _db.Pelanggan.Add(pelanggan);
            await _db.SaveChangesAsync();
            return pelanggan;
        }

        public async Task<Pelanggan?> UpdateAsync(Pelanggan pelanggan)
        {
            var existing = await _db.Pelanggan.FindAsync(pelanggan.Id);
            if (existing == null) return null;

            existing.Password = pelanggan.Password;
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pelanggan = await _db.Pelanggan.FindAsync(id);
            if (pelanggan == null) return false;

            _db.Pelanggan.Remove(pelanggan);
            await _db.SaveChangesAsync();
            return true;
        }

        // Method tambahan khusus Pelanggan
        public async Task<Pelanggan?> LoginAsync(string noTelp, string password)
        {
            return await _db.Pelanggan
                .FirstOrDefaultAsync(p => p.NoTelp == noTelp && p.Password == password);
        }

        public async Task<bool> NoTelpSudahAdaAsync(string noTelp)
        {
            return await _db.Pelanggan.AnyAsync(p => p.NoTelp == noTelp);
        }
    }
}
