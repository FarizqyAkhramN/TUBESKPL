using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PROJECTKPL.CONSOLE.Services
{
    public class PelangganService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json;

        public PelangganService(HttpClient http)
        {
            _http = http;
            _json = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<JsonElement>> GetAllAsync()
        {
            var res = await _http.GetAsync("api/pelanggan");
            if (!res.IsSuccessStatusCode) return new();
            var list = await res.Content.ReadFromJsonAsync<List<JsonElement>>(_json);
            return list ?? new();
        }

        public async Task<JsonElement?> LoginAsync(string noTelp, string password)
        {
            var res = await _http.PostAsJsonAsync("api/pelanggan/login", new { noTelp, password });
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<JsonElement>(_json);
        }

        public async Task TambahAsync(string username, string gender, string noTelp, int umur, string password)
        {
            var res = await _http.PostAsJsonAsync("api/pelanggan", new { username, gender, noTelp, umur, password });
            string msg = await res.Content.ReadAsStringAsync();
            System.Console.WriteLine(res.IsSuccessStatusCode
                ? $"Pelanggan '{username}' berhasil didaftarkan."
                : $"[ERROR] {msg}");
        }

        public async Task GantiPasswordAsync(int id, string noTelp, string passwordBaru)
        {
            var res = await _http.PutAsJsonAsync($"api/pelanggan/{id}/password", new { noTelp, passwordBaru });
            string msg = await res.Content.ReadAsStringAsync();
            System.Console.WriteLine(res.IsSuccessStatusCode ? msg : $"[ERROR] {msg}");
        }

        public async Task HapusAsync(int id)
        {
            var res = await _http.DeleteAsync($"api/pelanggan/{id}");
            string msg = await res.Content.ReadAsStringAsync();
            System.Console.WriteLine(res.IsSuccessStatusCode ? msg : $"[ERROR] {msg}");
        }

        public void PrintList(List<JsonElement> list)
        {
            if (list.Count == 0) { System.Console.WriteLine("Belum ada pelanggan."); return; }
            for (int i = 0; i < list.Count; i++)
            {
                var p = list[i];
                System.Console.WriteLine($"[{i + 1}] Id: {p.GetProperty("id")} | {p.GetProperty("username")} | {p.GetProperty("gender")} | {p.GetProperty("noTelp")} | {p.GetProperty("umur")} tahun");
            }
        }
    }
}
