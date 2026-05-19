using Microsoft.AspNetCore.Mvc;
using PROJECTKPL.API.Models;
using PROJECTKPL.API.Data;
using Microsoft.EntityFrameworkCore;

namespace PROJECTKPL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PesananController : ControllerBase
    {
        private readonly AppDbContext _db;
        private static int _counter = 1;

        public PesananController(AppDbContext db)
        {
            _db = db;
        }

        // GET api/pesanan
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Pesanan
                .Include(p => p.Pelanggan)
                .Include(p => p.Obat)
                .ToListAsync();
            return Ok(list);
        }

        // GET api/pesanan/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pesanan = await _db.Pesanan
                .Include(p => p.Pelanggan)
                .Include(p => p.Obat)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pesanan == null) return NotFound($"Pesanan id {id} tidak ditemukan.");
            return Ok(pesanan);
        }

        // GET api/pesanan/pelanggan/{pelangganId}
        [HttpGet("pelanggan/{pelangganId}")]
        public async Task<IActionResult> GetByPelanggan(int pelangganId)
        {
            var list = await _db.Pesanan
                .Include(p => p.Obat)
                .Where(p => p.PelangganId == pelangganId)
                .ToListAsync();
            return Ok(list);
        }

        // POST api/pesanan
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PesananRequest req)
        {
            var obat = await _db.Obat.FindAsync(req.ObatId);
            if (obat == null) return NotFound($"Obat id {req.ObatId} tidak ditemukan.");
            if (obat.StatusObat == StatusObat.Habis) return BadRequest($"Obat '{obat.NamaObat}' sedang habis.");
            if (obat.Stok < req.Jumlah) return BadRequest($"Stok tidak cukup. Tersedia: {obat.Stok}.");

            var pelanggan = await _db.Pelanggan.FindAsync(req.PelangganId);
            if (pelanggan == null) return NotFound($"Pelanggan id {req.PelangganId} tidak ditemukan.");

            var pesanan = new Pesanan
            {
                IdPesanan = "PSN" + _counter++,
                PelangganId = req.PelangganId,
                ObatId = req.ObatId,
                Jumlah = req.Jumlah,
                MetodePengambilan = req.MetodePengambilan,
                Pembayaran = req.Pembayaran,
                Status = StatusPesanan.Keranjang
            };

            obat.SetStok(obat.Stok - req.Jumlah);

            _db.Pesanan.Add(pesanan);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = pesanan.Id }, pesanan);
        }

        // PUT api/pesanan/{id}/trigger
        // Body: { "trigger": "Konfirmasi" }  (atau Siapkan, AmbilDanBayar, Batalkan)
        [HttpPut("{id}/trigger")]
        public async Task<IActionResult> AktifkanTrigger(int id, [FromBody] TriggerRequest req)
        {
            var pesanan = await _db.Pesanan.FindAsync(id);
            if (pesanan == null) return NotFound($"Pesanan id {id} tidak ditemukan.");

            var statusBaru = pesanan.AktifkanTrigger(req.Trigger);
            if (statusBaru == null)
                return BadRequest($"Transisi tidak valid: {pesanan.Status} + {req.Trigger}");

            await _db.SaveChangesAsync();
            return Ok(new { pesanan.Id, pesanan.IdPesanan, StatusBaru = statusBaru });
        }

        // DELETE api/pesanan/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pesanan = await _db.Pesanan.FindAsync(id);
            if (pesanan == null) return NotFound($"Pesanan id {id} tidak ditemukan.");

            _db.Pesanan.Remove(pesanan);
            await _db.SaveChangesAsync();
            return Ok($"Pesanan '{pesanan.IdPesanan}' berhasil dihapus.");
        }
    }

    // Request DTOs
    public record PesananRequest(
        int PelangganId,
        int ObatId,
        int Jumlah,
        MetodePengambilan MetodePengambilan,
        int Pembayaran
    );
    public record TriggerRequest(TriggerPesanan Trigger);
}
