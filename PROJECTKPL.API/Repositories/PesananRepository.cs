using Microsoft.EntityFrameworkCore;
using PROJECTKPL.API.Data;
using PROJECTKPL.API.Models;

namespace PROJECTKPL.API.Repositories
{
    public class PesananRepository : IRepository<Pesanan>
    {
        private readonly AppDbContext _db;

        public PesananRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Pesanan>> GetAllAsync()
        {
            return await _db.Pesanan
                .Include(p => p.Pelanggan)
                .Include(p => p.Obat)
                .ToListAsync();
        }

        public async Task<Pesanan?> GetByIdAsync(int id)
        {
            return await _db.Pesanan
                .Include(p => p.Pelanggan)
                .Include(p => p.Obat)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pesanan> AddAsync(Pesanan pesanan)
        {
            _db.Pesanan.Add(pesanan);
            await _db.SaveChangesAsync();
            return pesanan;
        }

        public async Task<Pesanan?> UpdateAsync(Pesanan pesanan)
        {
            var existing = await _db.Pesanan.FindAsync(pesanan.Id);
            if (existing == null) return null;

            existing.Status = pesanan.Status;
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pesanan = await _db.Pesanan.FindAsync(id);
            if (pesanan == null) return false;

            _db.Pesanan.Remove(pesanan);
            await _db.SaveChangesAsync();
            return true;
        }

        // Method tambahan khusus Pesanan
        public async Task<IEnumerable<Pesanan>> GetByPelangganAsync(int pelangganId)
        {
            return await _db.Pesanan
                .Include(p => p.Obat)
                .Where(p => p.PelangganId == pelangganId)
                .ToListAsync();
        }
    }
}
