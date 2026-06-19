using System.ComponentModel.DataAnnotations;

namespace PROJECTKPL.API.Models
{
    public enum StatusObat
    {
        Tersedia,
        HampirHabis,
        Habis
    }

    public class Obat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        private const int BATAS_HAMPIR_HABIS = 10;

        [Required]
        public string NamaObat { get; set; } = string.Empty;

        public int Stok { get; set; }

        public StatusObat StatusObat { get; set; }

        public int Harga { get; set; }

        public Obat() { }


        public Obat(string namaObat, int stok, int harga)
        {
            NamaObat = namaObat;
            Harga = harga;
            SetStok(stok);
        }

        // Table-driven: (batasAtas, status)
        private static readonly (int BatasAtas, StatusObat Status)[] TabelStatus =
        {
            (0,  StatusObat.Habis),
            (BATAS_HAMPIR_HABIS, StatusObat.HampirHabis),
            (int.MaxValue, StatusObat.Tersedia),
        };

        public void SetStok(int stokBaru)
        {
            Stok = stokBaru;
            foreach (var (batas, status) in TabelStatus)
            {
                if (stokBaru <= batas) { StatusObat = status; return; }
            }
        }
    }
}
