using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PROJECTKPL.CONSOLE.Services
{
    public class ObatService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json;

        public ObatService(HttpClient http)
        {
            _http = http;
            _json = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<dynamic>> GetAllAsync()
        {
            var res = await _http.GetAsync("api/obat");
            if (!res.IsSuccessStatusCode) return new();
            var list = await res.Content.ReadFromJsonAsync<List<dynamic>>(_json);
            return list ?? new();
        }

        public async Task TambahAsync(string namaObat, int stok, int harga)
        {
            var res = await _http.PostAsJsonAsync("api/obat", new { namaObat, stok, harga });
            string msg = await res.Content.ReadAsStringAsync();
            System.Console.WriteLine(res.IsSuccessStatusCode
                ? $"Obat '{namaObat}' berhasil ditambahkan."
                : $"[ERROR] {msg}");
        }

        public async Task EditStokAsync(int id, int stokBaru)
        {
            var res = await _http.PutAsJsonAsync($"api/obat/{id}/stok", new { stokBaru });
            string msg = await res.Content.ReadAsStringAsync();
            System.Console.WriteLine(res.IsSuccessStatusCode
                ? $"Stok berhasil diperbarui."
                : $"[ERROR] {msg}");
        }

        public async Task HapusAsync(int id)
        {
            var res = await _http.DeleteAsync($"api/obat/{id}");
            string msg = await res.Content.ReadAsStringAsync();
            System.Console.WriteLine(res.IsSuccessStatusCode ? msg : $"[ERROR] {msg}");
        }

        public void PrintList(List<dynamic> list)
        {
            if (list.Count == 0) { System.Console.WriteLine("Tidak ada obat."); return; }
            for (int i = 0; i < list.Count; i++)
            {
                var o = list[i] as JsonElement? ?? default;
                System.Console.WriteLine($"[{i + 1}] Id: {o.GetProperty("id")} | {o.GetProperty("namaObat")} | Stok: {o.GetProperty("stok")} ({o.GetProperty("statusObat")}) | Rp{o.GetProperty("harga")}");
            }
        }
    }

}
