using PROJECTKPL.API.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PROJECTKPL.CONSOLE.Services
{
    public class ObatService : BaseService<JsonElement>
    {
        public ObatService(HttpClient http) : base(http) { }

        public async Task<ApiResponse<List<JsonElement>>> GetAllAsync()
            => await GetListAsync("api/obat");

        public async Task<ApiResponse<JsonElement>> TambahAsync(string namaObat, int stok, int harga)
        {
            var result = await PostAsync("api/obat", new { namaObat, stok, harga });
            if (result.Success)
                System.Console.WriteLine($"Obat '{namaObat}' berhasil ditambahkan.");
            else
                System.Console.WriteLine($"[ERROR] {result.Message}");
            return result;
        }

        public async Task EditStokAsync(int id, int stokBaru)
        {
            var result = await PutAsync($"api/obat/{id}/stok", new { stokBaru });
            System.Console.WriteLine(result.Success ? "Stok berhasil diperbarui." : $"[ERROR] {result.Message}");
        }

        public async Task HapusAsync(int id)
        {
            var result = await DeleteAsync($"api/obat/{id}");
            System.Console.WriteLine(result.Success ? result.Data : $"[ERROR] {result.Message}");
        }

        public void PrintList(List<JsonElement> list)
        {
            if (list.Count == 0) { System.Console.WriteLine("Tidak ada obat."); return; }
            for (int i = 0; i < list.Count; i++)
            {
                var o = list[i];
                System.Console.WriteLine(
                    $"[{i + 1}] Id: {o.GetProperty("id")} | {o.GetProperty("namaObat")} " +
                    $"| Stok: {o.GetProperty("stok")} ({o.GetProperty("statusObat")}) " +
                    $"| Rp{o.GetProperty("harga")}");
            }
        }
    }
}