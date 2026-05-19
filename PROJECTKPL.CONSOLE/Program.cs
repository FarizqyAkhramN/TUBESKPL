using PROJECTKPL.CONSOLE.Menus;
using PROJECTKPL.CONSOLE.Services;
using System;
using System.Collections.Generic;
using System.Text;

var http = new HttpClient { BaseAddress = new Uri("http://localhost:5252/") };

var obatService = new ObatService(http);
var pelangganService = new PelangganService(http);
var pesananService = new PesananService(http);

const string ADMIN_USERNAME = "admin";
const string ADMIN_PASSWORD = "admin123";

// ── Menu Utama ──────────────────────────────────────────────────────────────
bool running = true;
while (running)
{
    Console.WriteLine("\n========================================");
    Console.WriteLine("     SELAMAT DATANG DI APOTEK KAMI");
    Console.WriteLine("========================================");
    Console.WriteLine("[1] Login sebagai Pramuniaga / Admin");
    Console.WriteLine("[2] Login sebagai Pelanggan");
    Console.WriteLine("[3] Daftar sebagai Pelanggan Baru");
    Console.WriteLine("[0] Keluar");
    Console.Write("Pilih: ");
    string pilihan = Console.ReadLine() ?? "";

    switch (pilihan)
    {
        case "1":
            await LoginPramuniaga();
            break;

        case "2":
            await LoginPelanggan();
            break;

        case "3":
            await DaftarPelanggan();
            break;

        case "0":
            Console.WriteLine("Terima kasih. Sampai jumpa!");
            running = false;
            break;

        default:
            Console.WriteLine("[ERROR] Pilihan tidak valid.");
            break;
    }
}

// ── Login Pramuniaga ────────────────────────────────────────────────────────
async Task LoginPramuniaga()
{
    Console.WriteLine("\n--- Login Pramuniaga ---");
    Console.Write("Username : ");
    string username = Console.ReadLine() ?? "";
    Console.Write("Password : ");
    string password = Console.ReadLine() ?? "";

    if (username != ADMIN_USERNAME || password != ADMIN_PASSWORD)
    {
        Console.WriteLine("[ERROR] Username atau password salah.");
        return;
    }

    Console.WriteLine($"\nSelamat datang, {username}!");
    var menu = new MenuPramuniaga(obatService, pelangganService, pesananService);
    await menu.TampilAsync();
}

// ── Login Pelanggan ─────────────────────────────────────────────────────────
async Task LoginPelanggan()
{
    Console.WriteLine("\n--- Login Pelanggan ---");
    Console.Write("No. Telepon : ");
    string noTelp = Console.ReadLine() ?? "";
    Console.Write("Password    : ");
    string password = Console.ReadLine() ?? "";

    var pelangganData = await pelangganService.LoginAsync(noTelp, password);
    if (pelangganData == null)
    {
        Console.WriteLine("[ERROR] No. telepon atau password salah.");
        return;
    }

    string username = pelangganData.Value.GetProperty("username").GetString() ?? "";
    Console.WriteLine($"\nSelamat datang, {username}!");

    var menu = new MenuPelanggan(obatService, pelangganService, pesananService, pelangganData.Value);
    await menu.TampilAsync();
}

async Task DaftarPelanggan()
{
    Console.WriteLine("\n--- Daftar Akun Baru ---");
    Console.Write("Username : ");
    string username = Console.ReadLine() ?? "";
    Console.Write("Gender   : ");
    string gender = Console.ReadLine() ?? "";
    Console.Write("No. Telp : ");
    string noTelp = Console.ReadLine() ?? "";
    Console.Write("Umur     : ");
    if (!int.TryParse(Console.ReadLine(), out int umur)) { Console.WriteLine("[ERROR] Input tidak valid."); return; }
    Console.Write("Password : ");
    string password = Console.ReadLine() ?? "";

    await pelangganService.TambahAsync(username, gender, noTelp, umur, password);
}