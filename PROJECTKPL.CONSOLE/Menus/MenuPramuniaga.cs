using PROJECTKPL.CONSOLE.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace PROJECTKPL.CONSOLE.Menus
{
    internal class MenuPramuniaga
    {
        private readonly ObatService _obatService;
        private readonly PelangganService _pelangganService;
        private readonly PesananService _pesananService;

        public MenuPramuniaga(ObatService obatService, PelangganService pelangganService, PesananService pesananService)
        {
            _obatService = obatService;
            _pelangganService = pelangganService;
            _pesananService = pesananService;
        }

        public async Task TampilAsync()
        {
            bool aktif = true;
            while (aktif)
            {
                System.Console.WriteLine("\n===== MENU PRAMUNIAGA =====");
                System.Console.WriteLine("[1] Kelola Obat");
                System.Console.WriteLine("[2] Kelola Pelanggan");
                System.Console.WriteLine("[3] Kelola Pesanan");
                System.Console.WriteLine("[0] Logout");
                System.Console.Write("Pilih: ");
                string pilihan = System.Console.ReadLine() ?? "";

                switch (pilihan)
                {
                    case "1": await MenuKelolaObat(); break;
                    case "2": await MenuKelolaPelanggan(); break;
                    case "3": await MenuKelolaPesanan(); break;
                    case "0":
                        System.Console.WriteLine("Logout berhasil.");
                        aktif = false;
                        break;
                    default:
                        System.Console.WriteLine("[ERROR] Pilihan tidak valid.");
                        break;
                }
            }
        }
        private async Task MenuKelolaObat()
        {
            bool aktif = true;
            while (aktif)
            {
                System.Console.WriteLine("\n----- Kelola Obat -----");
                System.Console.WriteLine("[1] Lihat Semua Obat");
                System.Console.WriteLine("[2] Tambah Obat");
                System.Console.WriteLine("[3] Edit Stok Obat");
                System.Console.WriteLine("[4] Hapus Obat");
                System.Console.WriteLine("[0] Kembali");
                System.Console.Write("Pilih: ");
                string pilihan = System.Console.ReadLine() ?? "";

                switch (pilihan)
                {
                    case "1":
                        var semua = await _obatService.GetAllAsync();
                        System.Console.WriteLine("\n===== DAFTAR OBAT =====");
                        _obatService.PrintList(semua);
                        break;

                    case "2":
                        System.Console.Write("Nama Obat  : ");
                        string nama = System.Console.ReadLine() ?? "";
                        System.Console.Write("Stok       : ");
                        if (!int.TryParse(System.Console.ReadLine(), out int stok)) { System.Console.WriteLine("[ERROR] Input tidak valid."); break; }
                        System.Console.Write("Harga (Rp) : ");
                        if (!int.TryParse(System.Console.ReadLine(), out int harga)) { System.Console.WriteLine("[ERROR] Input tidak valid."); break; }
                        await _obatService.TambahAsync(nama, stok, harga);
                        break;

                    case "3":
                        var listEdit = await _obatService.GetAllAsync();
                        System.Console.WriteLine("\n===== DAFTAR OBAT =====");
                        _obatService.PrintList(listEdit);
                        System.Console.Write("Id obat yang diedit: ");
                        if (!int.TryParse(System.Console.ReadLine(), out int idEdit)) { System.Console.WriteLine("[ERROR] Input tidak valid."); break; }
                        System.Console.Write("Stok Baru  : ");
                        if (!int.TryParse(System.Console.ReadLine(), out int stokBaru)) { System.Console.WriteLine("[ERROR] Input tidak valid."); break; }
                        await _obatService.EditStokAsync(idEdit, stokBaru);
                        break;

                    case "4":
                        var listHapus = await _obatService.GetAllAsync();
                        System.Console.WriteLine("\n===== DAFTAR OBAT =====");
                        _obatService.PrintList(listHapus);
                        System.Console.Write("Id obat yang dihapus: ");
                        if (!int.TryParse(System.Console.ReadLine(), out int idHapus)) { System.Console.WriteLine("[ERROR] Input tidak valid."); break; }
                        await _obatService.HapusAsync(idHapus);
                        break;

                    case "0":
                        aktif = false;
                        break;

                    default:
                        System.Console.WriteLine("[ERROR] Pilihan tidak valid.");
                        break;
                }
            }
        }

        private async Task MenuKelolaPesanan()
        {
            bool aktif = true;
            while (aktif)
            {
                System.Console.WriteLine("\n----- Kelola Pesanan -----");
                System.Console.WriteLine("[1] Lihat Semua Pesanan");
                System.Console.WriteLine("[2] Konfirmasi Pesanan");
                System.Console.WriteLine("[3] Siapkan Pesanan");
                System.Console.WriteLine("[4] Selesaikan Pesanan (Ambil & Bayar)");
                System.Console.WriteLine("[5] Batalkan Pesanan");
                System.Console.WriteLine("[0] Kembali");
                System.Console.Write("Pilih: ");
                string pilihan = System.Console.ReadLine() ?? "";

                switch (pilihan)
                {
                    case "1":
                        var list = await _pesananService.GetAllAsync();
                        _pesananService.PrintList(list, "SEMUA PESANAN");
                        break;

                    case "2": await AksiPesanan("Konfirmasi"); break;
                    case "3": await AksiPesanan("Siapkan"); break;
                    case "4": await AksiPesanan("AmbilDanBayar"); break;
                    case "5": await AksiPesanan("Batalkan"); break;

                    case "0":
                        aktif = false;
                        break;

                    default:
                        System.Console.WriteLine("[ERROR] Pilihan tidak valid.");
                        break;
                }
            }
        }

        // ── Kelola Pelanggan ───────────────────────────────────────────────────
        private async Task MenuKelolaPelanggan()
        {
            bool aktif = true;
            while (aktif)
            {
                System.Console.WriteLine("\n----- Kelola Pelanggan -----");
                System.Console.WriteLine("[1] Lihat Semua Pelanggan");
                System.Console.WriteLine("[2] Tambah Pelanggan");
                System.Console.WriteLine("[3] Hapus Pelanggan");
                System.Console.WriteLine("[0] Kembali");
                System.Console.Write("Pilih: ");
                string pilihan = System.Console.ReadLine() ?? "";

                switch (pilihan)
                {
                    case "1":
                        var list = await _pelangganService.GetAllAsync();
                        System.Console.WriteLine("\n===== DAFTAR PELANGGAN =====");
                        _pelangganService.PrintList(list);
                        break;

                    case "2":
                        System.Console.Write("Username : ");
                        string username = System.Console.ReadLine() ?? "";
                        System.Console.Write("Gender   : ");
                        string gender = System.Console.ReadLine() ?? "";
                        System.Console.Write("No. Telp : ");
                        string noTelp = System.Console.ReadLine() ?? "";
                        System.Console.Write("Umur     : ");
                        if (!int.TryParse(System.Console.ReadLine(), out int umur)) { System.Console.WriteLine("[ERROR] Input tidak valid."); break; }
                        System.Console.Write("Password : ");
                        string pass = System.Console.ReadLine() ?? "";
                        await _pelangganService.TambahAsync(username, gender, noTelp, umur, pass);
                        break;

                    case "3":
                        var listHapus = await _pelangganService.GetAllAsync();
                        System.Console.WriteLine("\n===== DAFTAR PELANGGAN =====");
                        _pelangganService.PrintList(listHapus);
                        System.Console.Write("Id pelanggan yang dihapus: ");
                        if (!int.TryParse(System.Console.ReadLine(), out int idHapus)) { System.Console.WriteLine("[ERROR] Input tidak valid."); break; }
                        await _pelangganService.HapusAsync(idHapus);
                        break;

                    case "0":
                        aktif = false;
                        break;

                    default:
                        System.Console.WriteLine("[ERROR] Pilihan tidak valid.");
                        break;
                }
            }
        }

        private async Task AksiPesanan(string trigger)
        {
            var list = await _pesananService.GetAllAsync();
            _pesananService.PrintList(list, "SEMUA PESANAN");
            if (list.Count == 0) return;

            System.Console.Write("Id pesanan: ");
            if (!int.TryParse(System.Console.ReadLine(), out int id))
            {
                System.Console.WriteLine("[ERROR] Input tidak valid.");
                return;
            }
            await _pesananService.AktifkanTriggerAsync(id, trigger);
        }

    }
}
