using System.Net.Http.Json;
using System.Text.Json;

namespace PROJECTKPL.GUI.Services
{
    // Facade Pattern — menyederhanakan komunikasi HTTP ke API
    public class PelangganService
    {
        private readonly HttpClient _http;

        public PelangganService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<JsonElement>> GetAllAsync()
        {
            try
            {
                var res = await _http.GetAsync("api/pelanggan");
                if (!res.IsSuccessStatusCode) return new();
                return await res.Content.ReadFromJsonAsync<List<JsonElement>>() ?? new();
            }
            catch { return new(); }
        }

        public async Task<(bool sukses, JsonElement? data)> LoginAsync(string noTelp, string password)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("api/pelanggan/login",
                    new { noTelp, password });
                if (!res.IsSuccessStatusCode) return (false, null);
                var data = await res.Content.ReadFromJsonAsync<JsonElement>();
                return (true, data);
            }
            catch { return (false, null); }
        }

        public async Task<(bool sukses, string pesan)> DaftarAsync(
            string username, string gender, string noTelp, int umur, string password)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("api/pelanggan",
                    new { username, gender, noTelp, umur, password });
                string msg = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, msg);
            }
            catch { return (false, "Tidak dapat terhubung ke server."); }
        }

        public async Task<(bool sukses, string pesan)> GantiPasswordAsync(
            int id, string noTelp, string passwordBaru)
        {
            try
            {
                var res = await _http.PutAsJsonAsync($"api/pelanggan/{id}/password",
                    new { noTelp, passwordBaru });
                string msg = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, msg);
            }
            catch { return (false, "Tidak dapat terhubung ke server."); }
        }

        public async Task<(bool sukses, string pesan)> HapusAsync(int id)
        {
            try
            {
                var res = await _http.DeleteAsync($"api/pelanggan/{id}");
                string msg = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, msg);
            }
            catch { return (false, "Tidak dapat terhubung ke server."); }
        }
    }
}