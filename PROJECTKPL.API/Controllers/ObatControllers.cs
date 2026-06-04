using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROJECTKPL.API.Data;
using PROJECTKPL.API.Models;
using PROJECTKPL.API.Validators;

namespace PROJECTKPL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObatController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ObatValidator _validator = new();

        public ObatController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var obat = await _db.Obat.ToListAsync();
            return Ok(obat);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var obat = await _db.Obat.FindAsync(id);
            if (obat == null) return NotFound($"Obat id {id} tidak ditemukan.");
            return Ok(obat);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ObatRequest req)
        {
            var obat = new Obat(req.NamaObat, req.Stok, req.Harga);

            // Validasi semua ruleset sekaligus
            var hasil = _validator.ValidateAll(obat);
            if (!hasil.IsValid)
            {
                var errors = hasil.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                return BadRequest(errors);
            }

            _db.Obat.Add(obat);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = obat.Id }, obat);
        }

        [HttpPut("{id}/stok")]
        public async Task<IActionResult> EditStok(int id, [FromBody] EditStokRequest req)
        {
            var obat = await _db.Obat.FindAsync(id);
            if (obat == null) return NotFound($"Obat id {id} tidak ditemukan.");

            // Validasi ruleset Stok saja
            obat.SetStok(req.StokBaru);
            var hasil = _validator.Validate(obat, "Stok");
            if (!hasil.IsValid)
            {
                var errors = hasil.Errors.Select(e => e.ErrorMessage);
                return BadRequest(errors);
            }

            await _db.SaveChangesAsync();
            return Ok(obat);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var obat = await _db.Obat.FindAsync(id);
            if (obat == null) return NotFound($"Obat id {id} tidak ditemukan.");

            _db.Obat.Remove(obat);
            await _db.SaveChangesAsync();
            return Ok($"Obat '{obat.NamaObat}' berhasil dihapus.");
        }
    }

    public record ObatRequest(string NamaObat, int Stok, int Harga);
    public record EditStokRequest(int StokBaru);

}
