using System.Net.Http.Json;
using System.Text.Json;

namespace PROJECTKPL.GUI.Services
{
    // Facade Pattern — menyederhanakan komunikasi HTTP ke API
    // Form tidak perlu tahu HttpClient, JSON parsing, atau error handling
    public class ObatService
    {
        private readonly HttpClient _http;

        public ObatService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<JsonElement>> GetAllAsync()
        {
            try
            {
                var res = await _http.GetAsync("api/obat");
                if (!res.IsSuccessStatusCode) return new();
                return await res.Content.ReadFromJsonAsync<List<JsonElement>>() ?? new();
            }
            catch { return new(); }
        }

        public async Task<(bool sukses, string pesan)> TambahAsync(string nama, int stok, int harga)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("api/obat",
                    new { namaObat = nama, stok, harga });
                string msg = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, msg);
            }
            catch { return (false, "Tidak dapat terhubung ke server."); }
        }

        public async Task<(bool sukses, string pesan)> EditAsync(int id, string nama, int stok, int harga)
        {
            try
            {
                var res = await _http.PutAsJsonAsync($"api/obat/{id}",
                    new { namaObat = nama, stokBaru = stok, harga });
                string msg = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, msg);
            }
            catch { return (false, "Tidak dapat terhubung ke server."); }
        }

        public async Task<(bool sukses, string pesan)> HapusAsync(int id)
        {
            try
            {
                var res = await _http.DeleteAsync($"api/obat/{id}");
                string msg = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, msg);
            }
            catch { return (false, "Tidak dapat terhubung ke server."); }
        }
    }
}