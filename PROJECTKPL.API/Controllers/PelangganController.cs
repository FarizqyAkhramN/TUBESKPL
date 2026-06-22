using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROJECTKPL.API.Data;
using PROJECTKPL.API.Helpers;
using PROJECTKPL.API.Models;
using PROJECTKPL.API.Repositories;
using PROJECTKPL.API.Validators;

namespace PROJECTKPL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PelangganController : ControllerBase
    {
        private readonly PelangganRepository _repo;
        private readonly PelangganValidator _validator = new();

        public PelangganController(PelangganRepository repo)
        {
            _repo = repo;
        }

        // GET api/pelanggan
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        // GET api/pelanggan/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return NotFound("Data tidak ditemukan.");
            return Ok(new { p.Id, p.Username, p.Gender, p.NoTelp, p.Umur });
        }

        // POST api/pelanggan/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            // Cari pelanggan by NoTelp dulu
            var p = await _repo.CariByNoTelpAsync(req.NoTelp);

            // Verifikasi hash SHA256 — tidak bandingkan string langsung
            if (p == null || !PasswordHelper.Verify(req.Password, p.Password))
                return Unauthorized("No. telepon atau password salah.");

            return Ok(new { p.Id, p.Username, p.Gender, p.NoTelp, p.Umur });
        }

        // POST api/pelanggan
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PelangganRequest req)
        {
            var pelanggan = new Pelanggan
            {
                Username = req.Username,
                Gender = req.Gender,
                NoTelp = req.NoTelp,
                Umur = req.Umur,
                // Hash password SHA256 sebelum disimpan ke database
                Password = PasswordHelper.Hash(req.Password)
            };

            // Validasi semua ruleset — validasi password sebelum di-hash
            var pelangganUntukValidasi = new Pelanggan
            {
                Username = req.Username,
                Gender = req.Gender,
                NoTelp = req.NoTelp,
                Umur = req.Umur,
                Password = req.Password // validasi pakai password asli, bukan hash
            };

            var hasil = _validator.ValidateAll(pelangganUntukValidasi);
            if (!hasil.IsValid)
            {
                var errors = hasil.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                return BadRequest(errors);
            }

            bool sudahAda = await _repo.NoTelpSudahAdaAsync(req.NoTelp);
            if (sudahAda) return Conflict($"No. telp '{req.NoTelp}' sudah terdaftar.");

            var created = await _repo.AddAsync(pelanggan);
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                new { created.Id, created.Username, created.Gender, created.NoTelp, created.Umur });
        }

        // PUT api/pelanggan/{id}/password
        [HttpPut("{id}/password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] GantiPasswordRequest req)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return NotFound("Data tidak ditemukan.");
            if (p.NoTelp != req.NoTelp) return BadRequest("Nomor telepon tidak cocok.");

            // Validasi password baru sebelum di-hash
            var dummy = new Pelanggan { Password = req.PasswordBaru };
            var hasil = _validator.Validate(dummy, "Password");
            if (!hasil.IsValid)
            {
                var errors = hasil.Errors.Select(e => e.ErrorMessage);
                return BadRequest(errors);
            }

            // Hash password baru sebelum disimpan
            p.Password = PasswordHelper.Hash(req.PasswordBaru);
            await _repo.UpdateAsync(p);
            return Ok("Password berhasil diubah.");
        }

        // DELETE api/pelanggan/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return NotFound("Data tidak ditemukan.");

            string nama = p.Username;
            await _repo.DeleteAsync(id);
            return Ok($"Pelanggan '{nama}' berhasil dihapus.");
        }
    }

    public record LoginRequest(string NoTelp, string Password);
    public record PelangganRequest(string Username, string Gender, string NoTelp, int Umur, string Password);
    public record GantiPasswordRequest(string NoTelp, string PasswordBaru);
}
