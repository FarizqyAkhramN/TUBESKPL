using System;
using System.Collections.Generic;
using System.Text;

namespace PROJECTKPL.CONSOLE.Config
{
    public enum RoleUser
    {
        Pramuniaga,
        Pelanggan
    }

    public class MenuConfig
    {
        public string Label { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
    }

    // Konfigurasi yang dimuat saat runtime berdasarkan role user yang login
    public class AppConfig
    {
        public RoleUser Role { get; private set; }
        public string NamaUser { get; private set; } = string.Empty;
        public List<MenuConfig> MenuTersedia { get; private set; } = new();

        // Tabel konfigurasi menu per role — dimuat saat runtime
        private static readonly Dictionary<RoleUser, List<MenuConfig>> TabelMenu = new()
        {
            {
                RoleUser.Pramuniaga, new List<MenuConfig>
                {
                    new() { Key = "1", Label = "Kelola Obat"      },
                    new() { Key = "2", Label = "Kelola Pelanggan" },
                    new() { Key = "3", Label = "Kelola Pesanan"   },
                    new() { Key = "0", Label = "Logout"           },
                }
            },
            {
                RoleUser.Pelanggan, new List<MenuConfig>
                {
                    new() { Key = "1", Label = "Lihat Daftar Obat"      },
                    new() { Key = "2", Label = "Pesan Obat"             },
                    new() { Key = "3", Label = "Lihat Riwayat Pembelian"},
                    new() { Key = "4", Label = "Ganti Password"         },
                    new() { Key = "0", Label = "Logout"                 },
                }
            }
        };

        // Load konfigurasi saat runtime berdasarkan role
        public void Load(RoleUser role, string namaUser)
        {
            Role = role;
            NamaUser = namaUser;
            MenuTersedia = TabelMenu[role];

            System.Console.WriteLine($"\n[Config] Role '{role}' dimuat — {MenuTersedia.Count - 1} menu tersedia.");
        }

        public void TampilMenu()
        {
            System.Console.WriteLine($"\n===== MENU {Role.ToString().ToUpper()} ({NamaUser}) =====");
            foreach (var menu in MenuTersedia)
            {
                System.Console.WriteLine($"[{menu.Key}] {menu.Label}");
            }
            System.Console.Write("Pilih: ");
        }
    }
}
