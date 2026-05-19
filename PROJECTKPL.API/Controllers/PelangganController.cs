using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROJECTKPL.API.Data;
using PROJECTKPL.API.Models;
using PROJECTKPL.API.Validators;

namespace PROJECTKPL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PelangganController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly PelangganValidator _validator = new();

        public PelangganController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Pelanggan
                .Select(p => new { p.Id, p.Username, p.Gender, p.NoTelp, p.Umur })
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _db.Pelanggan.FindAsync(id);
            if (p == null) return NotFound($"Pelanggan id {id} tidak ditemukan.");
            return Ok(new { p.Id, p.Username, p.Gender, p.NoTelp, p.Umur });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var p = await _db.Pelanggan
                .FirstOrDefaultAsync(x => x.NoTelp == req.NoTelp && x.Password == req.Password);
            if (p == null) return Unauthorized("No. telepon atau password salah.");
            return Ok(new { p.Id, p.Username, p.Gender, p.NoTelp, p.Umur });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PelangganRequest req)
        {
            var pelanggan = new Pelanggan
            {
                Username = req.Username,
                Gender = req.Gender,
                NoTelp = req.NoTelp,
                Umur = req.Umur,
                Password = req.Password
            };

            // Validasi semua ruleset sekaligus
            var hasil = _validator.ValidateAll(pelanggan);
            if (!hasil.IsValid)
            {
                var errors = hasil.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                return BadRequest(errors);
            }

            bool sudahAda = await _db.Pelanggan.AnyAsync(p => p.NoTelp == req.NoTelp);
            if (sudahAda) return Conflict($"No. telp '{req.NoTelp}' sudah terdaftar.");

            _db.Pelanggan.Add(pelanggan);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = pelanggan.Id },
                new { pelanggan.Id, pelanggan.Username, pelanggan.Gender, pelanggan.NoTelp, pelanggan.Umur });
        }

        [HttpPut("{id}/password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] GantiPasswordRequest req)
        {
            var p = await _db.Pelanggan.FindAsync(id);
            if (p == null) return NotFound($"Pelanggan id {id} tidak ditemukan.");
            if (p.NoTelp != req.NoTelp) return BadRequest("Nomor telepon tidak cocok.");

            // Validasi ruleset Password saja
            p.Password = req.PasswordBaru;
            var hasil = _validator.Validate(p, "Password");
            if (!hasil.IsValid)
            {
                var errors = hasil.Errors.Select(e => e.ErrorMessage);
                return BadRequest(errors);
            }

            await _db.SaveChangesAsync();
            return Ok("Password berhasil diubah.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Pelanggan.FindAsync(id);
            if (p == null) return NotFound($"Pelanggan id {id} tidak ditemukan.");

            _db.Pelanggan.Remove(p);
            await _db.SaveChangesAsync();
            return Ok($"Pelanggan '{p.Username}' berhasil dihapus.");
        }
    }

    public record LoginRequest(string NoTelp, string Password);
    public record PelangganRequest(string Username, string Gender, string NoTelp, int Umur, string Password);
    public record GantiPasswordRequest(string NoTelp, string PasswordBaru);
}
