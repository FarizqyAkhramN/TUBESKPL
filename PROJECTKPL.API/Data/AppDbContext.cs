using PROJECTKPL.API.Models;
using Microsoft.EntityFrameworkCore;


namespace PROJECTKPL.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Obat> Obat { get; set; }
        public DbSet<Pelanggan> Pelanggan { get; set; }
        public DbSet<Pesanan> Pesanan { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed data awal obat
            modelBuilder.Entity<Obat>().HasData(
                new Obat { Id = 1, NamaObat = "Paracetamol 500mg", Stok = 50, StatusObat = StatusObat.Tersedia, Harga = 5000 },
                new Obat { Id = 2, NamaObat = "Amoxicillin 250mg", Stok = 8, StatusObat = StatusObat.HampirHabis, Harga = 12000 },
                new Obat { Id = 3, NamaObat = "Antasida", Stok = 0, StatusObat = StatusObat.Habis, Harga = 3000 }
            );

            // Seed pramuniaga/admin disimpan di tabel Pelanggan dengan flag khusus
            // (atau bisa pakai tabel terpisah — tergantung kebutuhan)
        }
    }
}
