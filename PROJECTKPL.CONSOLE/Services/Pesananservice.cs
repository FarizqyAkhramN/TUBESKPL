using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PROJECTKPL.CONSOLE.Services
{
    internal class Pesananservice
    {
        public class PesananService
        {
            private readonly HttpClient _http;
            private readonly JsonSerializerOptions _json;

            public PesananService(HttpClient http)
            {
                _http = http;
                _json = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            }

            public async Task<List<JsonElement>> GetAllAsync()
            {
                var res = await _http.GetAsync("api/pesanan");
                if (!res.IsSuccessStatusCode) return new();
                var list = await res.Content.ReadFromJsonAsync<List<JsonElement>>(_json);
                return list ?? new();
            }

            public async Task<List<JsonElement>> GetByPelangganAsync(int pelangganId)
            {
                var res = await _http.GetAsync($"api/pesanan/pelanggan/{pelangganId}");
                if (!res.IsSuccessStatusCode) return new();
                var list = await res.Content.ReadFromJsonAsync<List<JsonElement>>(_json);
                return list ?? new();
            }

            public async Task BuatPesananAsync(int pelangganId, int obatId, int jumlah, string metode, int pembayaran)
            {
                var res = await _http.PostAsJsonAsync("api/pesanan", new
                {
                    pelangganId,
                    obatId,
                    jumlah,
                    metodePengambilan = metode,
                    pembayaran
                });
                string msg = await res.Content.ReadAsStringAsync();
                System.Console.WriteLine(res.IsSuccessStatusCode
                    ? "Pesanan berhasil dibuat."
                    : $"[ERROR] {msg}");
            }

            public async Task AktifkanTriggerAsync(int pesananId, string trigger)
            {
                var res = await _http.PutAsJsonAsync($"api/pesanan/{pesananId}/trigger", new { trigger });
                string msg = await res.Content.ReadAsStringAsync();
                System.Console.WriteLine(res.IsSuccessStatusCode
                    ? $"Status berhasil diperbarui."
                    : $"[ERROR] {msg}");
            }

            public async Task HapusAsync(int id)
            {
                var res = await _http.DeleteAsync($"api/pesanan/{id}");
                string msg = await res.Content.ReadAsStringAsync();
                System.Console.WriteLine(res.IsSuccessStatusCode ? msg : $"[ERROR] {msg}");
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
}
