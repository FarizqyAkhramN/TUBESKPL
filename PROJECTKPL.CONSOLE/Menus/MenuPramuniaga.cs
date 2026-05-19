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
    }
}
