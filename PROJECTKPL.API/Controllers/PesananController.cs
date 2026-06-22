using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROJECTKPL.API.Data;
using PROJECTKPL.API.Models;
using PROJECTKPL.API.Repositories;

namespace PROJECTKPL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PesananController : ControllerBase
    {
        private readonly PesananRepository _repo;
        private readonly ObatRepository _repoObat;
        private static int _counter = 1;

        public PesananController(PesananRepository repo, ObatRepository repoObat)
        {
            _repo = repo;
            _repoObat = repoObat;
        }

        // GET api/pesanan
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        // GET api/pesanan/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pesanan = await _repo.GetByIdAsync(id);
            if (pesanan == null) return NotFound("Data tidak ditemukan.");
            return Ok(pesanan);
        }

        // GET api/pesanan/pelanggan/{pelangganId}
        [HttpGet("pelanggan/{pelangganId}")]
        public async Task<IActionResult> GetByPelanggan(int pelangganId)
        {
            var list = await _repo.GetByPelangganAsync(pelangganId);
            return Ok(list);
        }

        // POST api/pesanan
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PesananRequest req)
        {
            var obat = await _repoObat.GetByIdAsync(req.ObatId);
            if (obat == null) return NotFound("Obat tidak ditemukan.");
            if (obat.StatusObat == StatusObat.Habis)
                return BadRequest($"Obat '{obat.NamaObat}' sedang habis.");
            if (obat.Stok < req.Jumlah)
                return BadRequest($"Stok tidak cukup. Tersedia: {obat.Stok}.");

            // Validasi pembayaran mencukupi total harga
            int totalHarga = obat.Harga * req.Jumlah;
            if (req.Pembayaran < totalHarga)
                return BadRequest($"Pembayaran kurang. Total: Rp{totalHarga}, dibayar: Rp{req.Pembayaran}.");

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

            // Kurangi stok obat
            obat.SetStok(obat.Stok - req.Jumlah);
            await _repoObat.UpdateAsync(obat);

            var created = await _repo.AddAsync(pesanan);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/pesanan/{id}/trigger
        [HttpPut("{id}/trigger")]
        public async Task<IActionResult> AktifkanTrigger(int id, [FromBody] TriggerRequest req)
        {
            var pesanan = await _repo.GetByIdAsync(id);
            if (pesanan == null) return NotFound("Data tidak ditemukan.");

            var statusBaru = pesanan.AktifkanTrigger(req.Trigger);
            if (statusBaru == null)
                return BadRequest($"Transisi tidak valid: {pesanan.Status} + {req.Trigger}");

            await _repo.UpdateAsync(pesanan);
            return Ok(new { pesanan.Id, pesanan.IdPesanan, StatusBaru = statusBaru });
        }

        // DELETE api/pesanan/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pesanan = await _repo.GetByIdAsync(id);
            if (pesanan == null) return NotFound("Data tidak ditemukan.");

            string idPesanan = pesanan.IdPesanan;
            await _repo.DeleteAsync(id);
            return Ok($"Pesanan '{idPesanan}' berhasil dihapus.");
        }
    }

    public record PesananRequest(
        int PelangganId,
        int ObatId,
        int Jumlah,
        MetodePengambilan MetodePengambilan,
        int Pembayaran
    );
    public record TriggerRequest(TriggerPesanan Trigger);
}
