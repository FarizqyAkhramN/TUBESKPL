using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using PROJECTKPL.API.Models;

namespace PROJECTKPL.CONSOLE.Services
{
    public class PesananService : BaseService<JsonElement>
    {
        public PesananService(HttpClient http) : base(http) { }

        public async Task<ApiResponse<List<JsonElement>>> GetAllAsync()
            => await GetListAsync("api/pesanan");

        public async Task<ApiResponse<List<JsonElement>>> GetByPelangganAsync(int pelangganId)
            => await GetListAsync($"api/pesanan/pelanggan/{pelangganId}");

        public async Task BuatPesananAsync(int pelangganId, int obatId, int jumlah, string metode, int pembayaran)
        {
            var result = await PostAsync("api/pesanan", new { pelangganId, obatId, jumlah, metodePengambilan = metode, pembayaran });
            System.Console.WriteLine(result.Success ? "Pesanan berhasil dibuat." : $"[ERROR] {result.Message}");
        }

        public async Task AktifkanTriggerAsync(int pesananId, string trigger)
        {
            var result = await PutAsync($"api/pesanan/{pesananId}/trigger", new { trigger });
            System.Console.WriteLine(result.Success ? "Status berhasil diperbarui." : $"[ERROR] {result.Message}");
        }

        public async Task HapusAsync(int id)
        {
            var result = await DeleteAsync($"api/pesanan/{id}");
            System.Console.WriteLine(result.Success ? result.Data : $"[ERROR] {result.Message}");
        }

        public void PrintList(List<JsonElement> list, string header = "DAFTAR PESANAN")
        {
            System.Console.WriteLine($"\n===== {header} =====");
            if (list.Count == 0) { System.Console.WriteLine("Tidak ada pesanan."); return; }
            for (int i = 0; i < list.Count; i++)
            {
                var p = list[i];
                string namaObat = "-";
                if (p.TryGetProperty("obat", out var obat) && obat.ValueKind != JsonValueKind.Null)
                    namaObat = obat.GetProperty("namaObat").GetString() ?? "-";

                System.Console.WriteLine(
                    $"[{i + 1}] Id: {p.GetProperty("id")} | {p.GetProperty("idPesanan")} " +
                    $"| {namaObat} x{p.GetProperty("jumlah")} " +
                    $"| {p.GetProperty("metodePengambilan")} " +
                    $"| Rp{p.GetProperty("pembayaran")} " +
                    $"| Status: {p.GetProperty("status")}");
            }
        }
    }
}
