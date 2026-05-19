using PROJECTKPL.CONSOLE.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PROJECTKPL.CONSOLE.Menus
{
    internal class MenuPelanggan
    {
        private readonly ObatService _obatService;
        private readonly PelangganService _pelangganService;
        private readonly Pesanan  _pesananService;

        // Data pelanggan yang sedang login (dari response API)
        private readonly int _pelangganId;
        private readonly string _username;
        private readonly string _noTelp;

        public MenuPelanggan(
            ObatService obatService,
            PelangganService pelangganService,
            PesananService pesananService,
            JsonElement pelangganData)
        {
            _obatService = obatService;
            _pelangganService = pelangganService;
            _pesananService = pesananService;

            _pelangganId = pelangganData.GetProperty("id").GetInt32();
            _username = pelangganData.GetProperty("username").GetString() ?? "";
            _noTelp = pelangganData.GetProperty("noTelp").GetString() ?? "";
        }

        public async Task TampilAsync()
        {
            bool aktif = true;
            while (aktif)
            {
                System.Console.WriteLine($"\n===== MENU PELANGGAN ({_username}) =====");
                System.Console.WriteLine("[1] Lihat Daftar Obat");
                System.Console.WriteLine("[2] Pesan Obat");
                System.Console.WriteLine("[3] Lihat Riwayat Pembelian");
                System.Console.WriteLine("[4] Ganti Password");
                System.Console.WriteLine("[0] Logout");
                System.Console.Write("Pilih: ");
                string pilihan = System.Console.ReadLine() ?? "";

                switch (pilihan)
                {
                    case "1":
                        var obatList = await _obatService.GetAllAsync();
                        System.Console.WriteLine("\n===== DAFTAR OBAT =====");
                        _obatService.PrintList(obatList);
                        break;

                    case "2":
                        await MenuPesanObat();
                        break;

                    case "3":
                        var riwayat = await _pesananService.GetByPelangganAsync(_pelangganId);
                        _pesananService.PrintList(riwayat, $"RIWAYAT PEMBELIAN ({_username})");
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
