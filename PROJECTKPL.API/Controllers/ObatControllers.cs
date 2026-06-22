using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROJECTKPL.API.Data;
using PROJECTKPL.API.Models;
using PROJECTKPL.API.Repositories;
using PROJECTKPL.API.Validators;

namespace PROJECTKPL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ObatController : ControllerBase
    {
        private readonly ObatRepository _repo;
        private readonly ObatValidator _validator = new();

        public ObatController(ObatRepository repo)
        {
            _repo = repo;
        }

        // GET api/obat
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var obat = await _repo.GetAllAsync();
            return Ok(obat);
        }

        // GET api/obat/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var obat = await _repo.GetByIdAsync(id);
            if (obat == null) return NotFound("Data tidak ditemukan.");
            return Ok(obat);
        }

        // POST api/obat
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ObatRequest req)
        {
            var obat = new Obat(req.NamaObat, req.Stok, req.Harga);

            // Validasi semua ruleset
            var hasil = _validator.ValidateAll(obat);
            if (!hasil.IsValid)
            {
                var errors = hasil.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                return BadRequest(errors);
            }

            var created = await _repo.AddAsync(obat);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/obat/{id}/stok
        [HttpPut("{id}/stok")]
        public async Task<IActionResult> EditStok(int id, [FromBody] EditStokRequest req)
        {
            var obat = await _repo.GetByIdAsync(id);
            if (obat == null) return NotFound("Data tidak ditemukan.");

            // Validasi ruleset Stok saja
            if (req.StokBaru < 0)
                return BadRequest("Stok tidak boleh negatif.");

            obat.SetStok(req.StokBaru);
            var updated = await _repo.UpdateAsync(obat);
            return Ok(updated);
        }

        // DELETE api/obat/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var obat = await _repo.GetByIdAsync(id);
            if (obat == null) return NotFound("Data tidak ditemukan.");

            string nama = obat.NamaObat;
            await _repo.DeleteAsync(id);
            return Ok($"Obat '{nama}' berhasil dihapus.");
        }
    }

    public record ObatRequest(string NamaObat, int Stok, int Harga);
    public record EditStokRequest(int StokBaru);
}
