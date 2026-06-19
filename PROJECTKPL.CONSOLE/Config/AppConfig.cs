using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PROJECTKPL.CONSOLE.Config
{
    public enum RoleUser
    {
        Pramuniaga,
        Pelanggan
    }

    public class MenuConfig
    {
        public string Key { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    // Runtime Configuration:
    // Menu dibaca dari appsettings.json saat runtime.
    // Tidak perlu recompile jika menu berubah — cukup edit file JSON.
    public class AppConfig
    {
        public RoleUser Role { get; private set; }
        public string NamaUser { get; private set; } = string.Empty;
        public List<MenuConfig> MenuTersedia { get; private set; } = new();

        private static readonly string ConfigPath =
            Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        // Baca konfigurasi menu dari appsettings.json berdasarkan role
        public void Load(RoleUser role, string namaUser)
        {
            Role = role;
            NamaUser = namaUser;

            // Baca file JSON saat runtime
            if (!File.Exists(ConfigPath))
            {
                System.Console.WriteLine($"[ERROR] File konfigurasi tidak ditemukan: {ConfigPath}");
                return;
            }

            string json = File.ReadAllText(ConfigPath);
            using JsonDocument doc = JsonDocument.Parse(json);

            // Ambil section MenuConfig -> {role}
            string roleKey = role.ToString(); // "Pramuniaga" atau "Pelanggan"
            JsonElement menuSection = doc.RootElement
                .GetProperty("MenuConfig")
                .GetProperty(roleKey);

            MenuTersedia = JsonSerializer.Deserialize<List<MenuConfig>>(
                menuSection.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new();

            System.Console.WriteLine(
                $"\n[Config] Konfigurasi role '{roleKey}' dimuat dari appsettings.json " +
                $"— {MenuTersedia.Count - 1} menu tersedia.");
        }

        public void TampilMenu()
        {
            System.Console.WriteLine($"\n===== MENU {Role.ToString().ToUpper()} ({NamaUser}) =====");
            foreach (var menu in MenuTersedia)
                System.Console.WriteLine($"[{menu.Key}] {menu.Label}");
            System.Console.Write("Pilih: ");
        }
    }

}
