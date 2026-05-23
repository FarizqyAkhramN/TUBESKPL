using PROJECTKPL.API.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PROJECTKPL.CONSOLE.Services
{
    public class PelangganService : BaseService<JsonElement>
    {
        public PelangganService(HttpClient http) : base(http) { }

        public async Task<ApiResponse<List<JsonElement>>> GetAllAsync()
            => await GetListAsync("api/pelanggan");

        public async Task<ApiResponse<JsonElement>> LoginAsync(string noTelp, string password)
            => await PostAsync("api/pelanggan/login", new { noTelp, password });

        public async Task TambahAsync(string username, string gender, string noTelp, int umur, string password)
        {
            var result = await PostAsync("api/pelanggan", new { username, gender, noTelp, umur, password });
            System.Console.WriteLine(result.Success
                ? $"Pelanggan '{username}' berhasil didaftarkan."
                : $"[ERROR] {result.Message}");
        }

        public async Task GantiPasswordAsync(int id, string noTelp, string passwordBaru)
        {
            var result = await PutAsync($"api/pelanggan/{id}/password", new { noTelp, passwordBaru });
            System.Console.WriteLine(result.Success ? result.Data : $"[ERROR] {result.Message}");
        }

        public async Task HapusAsync(int id)
        {
            var result = await DeleteAsync($"api/pelanggan/{id}");
            System.Console.WriteLine(result.Success ? result.Data : $"[ERROR] {result.Message}");
        }

        public void PrintList(List<JsonElement> list)
        {
            if (list.Count == 0) { System.Console.WriteLine("Belum ada pelanggan."); return; }
            for (int i = 0; i < list.Count; i++)
            {
                var p = list[i];
                System.Console.WriteLine(
                    $"[{i + 1}] Id: {p.GetProperty("id")} | {p.GetProperty("username")} " +
                    $"| {p.GetProperty("gender")} | {p.GetProperty("noTelp")} " +
                    $"| {p.GetProperty("umur")} tahun");
            }
        }
    }
}
