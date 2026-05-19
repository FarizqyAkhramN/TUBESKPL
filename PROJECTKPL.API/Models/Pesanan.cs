using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROJECTKPL.API.Models
{
    public enum MetodePengambilan
    {
        Langsung,
        Diantar
    }

    public enum StatusPesanan
    {
        Keranjang,
        Diproses,
        Disiapkan,
        Selesai,
        Dibatalkan
    }

    public enum TriggerPesanan
    {
        Konfirmasi,
        Siapkan,
        AmbilDanBayar,
        Batalkan
    }

    public class Pesanan
    {
        [Key]
        public int Id { get; set; }

        public string IdPesanan { get; set; } = string.Empty;

        // FK ke Pelanggan
        public int PelangganId { get; set; }
        [ForeignKey("PelangganId")]
        public Pelanggan? Pelanggan { get; set; }

        // FK ke Obat
        public int ObatId { get; set; }
        [ForeignKey("ObatId")]
        public Obat? Obat { get; set; }

        public int Jumlah { get; set; }

        public MetodePengambilan MetodePengambilan { get; set; }

        public int Pembayaran { get; set; }

        public StatusPesanan Status { get; set; } = StatusPesanan.Keranjang;

        // State machine: daftar transisi valid
        private static readonly (StatusPesanan Dari, StatusPesanan Ke, TriggerPesanan Trigger)[] Transisi =
        {
            (StatusPesanan.Keranjang, StatusPesanan.Diproses,   TriggerPesanan.Konfirmasi),
            (StatusPesanan.Diproses,  StatusPesanan.Disiapkan,  TriggerPesanan.Siapkan),
            (StatusPesanan.Disiapkan, StatusPesanan.Selesai,    TriggerPesanan.AmbilDanBayar),
            (StatusPesanan.Keranjang, StatusPesanan.Dibatalkan, TriggerPesanan.Batalkan),
            (StatusPesanan.Diproses,  StatusPesanan.Dibatalkan, TriggerPesanan.Batalkan),
        };

        // Kembalikan status baru jika trigger valid, null jika tidak valid
        public StatusPesanan? AktifkanTrigger(TriggerPesanan trigger)
        {
            foreach (var t in Transisi)
            {
                if (t.Dari == Status && t.Trigger == trigger)
                {
                    Status = t.Ke;
                    return Status;
                }
            }
            return null; // transisi tidak valid
        }
    }
}
