using PROJECTKPL.CONSOLE.Config;
using PROJECTKPL.CONSOLE.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PROJECTKPL.CONSOLE.Menus
{
    public class MenuPelanggan
    {
        private readonly ObatService _obatService;
        private readonly PelangganService _pelangganService;
        private readonly PesananService _pesananService;
        private readonly AppConfig _config;

        private readonly int _pelangganId;
        private readonly string _noTelp;

        public MenuPelanggan(ObatService obatService, PelangganService pelangganService,
            PesananService pesananService, JsonElement pelangganData, AppConfig config)
        {
            _obatService = obatService;
            _pelangganService = pelangganService;
            _pesananService = pesananService;
            _config = config;

            _pelangganId = pelangganData.GetProperty("id").GetInt32();
            _noTelp = pelangganData.GetProperty("noTelp").GetString() ?? "";
        }

        public async Task TampilAsync()
        {
            bool aktif = true;
            while (aktif)
            {
                // Runtime config menampilkan menu sesuai role Pelanggan
                _config.TampilMenu();
                string pilihan = System.Console.ReadLine() ?? "";

                switch (pilihan)
                {
                    case "1":
                        var obatList = await _obatService.GetAllAsync();
                        System.Console.WriteLine("\n===== DAFTAR OBAT =====");
                        if (obatList.Success) _obatService.PrintList(obatList.Data!);
                        else System.Console.WriteLine($"[ERROR] {obatList.Message}");
                        break;
                    case "2":
                        await MenuPesanObat();
                        break;
                    case "3":
                        var riwayat = await _pesananService.GetByPelangganAsync(_pelangganId);
                        if (riwayat.Success) _pesananService.PrintList(riwayat.Data!, $"RIWAYAT PEMBELIAN");
                        else System.Console.WriteLine($"[ERROR] {riwayat.Message}");
                        break;
                    case "4":
                        System.Console.Write("No. Telepon (konfirmasi) : ");
                        string noTelp = System.Console.ReadLine() ?? "";
                        System.Console.Write("Password Baru            : ");
                        string passBaru = System.Console.ReadLine() ?? "";
                        await _pelangganService.GantiPasswordAsync(_pelangganId, noTelp, passBaru);
                        break;
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
        private async Task MenuPesanObat()
        {
            var obatList = await _obatService.GetAllAsync();
            System.Console.WriteLine("\n===== DAFTAR OBAT =====");
            if (!obatList.Success || obatList.Data!.Count == 0)
            {
                System.Console.WriteLine("Tidak ada obat tersedia.");
                return;
            }
            _obatService.PrintList(obatList.Data!);

            System.Console.Write("\nId Obat         : ");
            if (!int.TryParse(System.Console.ReadLine(), out int obatId)) { System.Console.WriteLine("[ERROR] Input tidak valid."); return; }

            System.Console.Write("Jumlah          : ");
            if (!int.TryParse(System.Console.ReadLine(), out int jumlah) || jumlah <= 0) { System.Console.WriteLine("[ERROR] Jumlah tidak valid."); return; }

            System.Console.WriteLine("Metode Pengambilan:");
            System.Console.WriteLine("[1] Langsung");
            System.Console.WriteLine("[2] Diantar");
            System.Console.Write("Pilih: ");
            string metode = (System.Console.ReadLine() ?? "") switch
            {
                "1" => "Langsung",
                "2" => "Diantar",
                _ => ""
            };
            if (metode == "") { System.Console.WriteLine("[ERROR] Pilihan metode tidak valid."); return; }

            System.Console.Write("Pembayaran (Rp) : ");
            if (!int.TryParse(System.Console.ReadLine(), out int pembayaran) || pembayaran <= 0) { System.Console.WriteLine("[ERROR] Nominal tidak valid."); return; }

            await _pesananService.BuatPesananAsync(_pelangganId, obatId, jumlah, metode, pembayaran);
        }
    }
}
