using System.ComponentModel.DataAnnotations;

namespace PROJECTKPL.API.Models
{
    public class Pelanggan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public string NoTelp { get; set; } = string.Empty;

        public int Umur { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty;

        // Navigasi ke pesanan milik pelanggan ini
        public List<Pesanan> RiwayatPembelian { get; set; } = new();
    }
}
