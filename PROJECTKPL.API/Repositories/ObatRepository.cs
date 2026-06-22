using Microsoft.EntityFrameworkCore;
using PROJECTKPL.API.Data;
using PROJECTKPL.API.Models;

namespace PROJECTKPL.API.Repositories
{
    public class ObatRepository : IRepository<Obat>
    {
        private readonly AppDbContext _db;

        public ObatRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Obat>> GetAllAsync()
        {
            return await _db.Obat.ToListAsync();
        }

        public async Task<Obat?> GetByIdAsync(int id)
        {
            return await _db.Obat.FindAsync(id);
        }

        public async Task<Obat> AddAsync(Obat obat)
        {
            _db.Obat.Add(obat);
            await _db.SaveChangesAsync();
            return obat;
        }

        public async Task<Obat?> UpdateAsync(Obat obat)
        {
            var existing = await _db.Obat.FindAsync(obat.Id);
            if (existing == null) return null;

            // Update semua field
            existing.NamaObat = obat.NamaObat;
            existing.Harga = obat.Harga;
            existing.SetStok(obat.Stok);

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var obat = await _db.Obat.FindAsync(id);
            if (obat == null) return false;

            _db.Obat.Remove(obat);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
