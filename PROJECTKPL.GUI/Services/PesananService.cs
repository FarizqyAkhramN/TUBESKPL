using System.Net.Http.Json;
using System.Text.Json;

namespace PROJECTKPL.GUI.Services
{
    // Facade Pattern — menyederhanakan komunikasi HTTP ke API
    public class PesananService
    {
        private readonly HttpClient _http;

        public PesananService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<JsonElement>> GetAllAsync()
        {
            try
            {
                var res = await _http.GetAsync("api/pesanan");
                if (!res.IsSuccessStatusCode) return new();
                return await res.Content.ReadFromJsonAsync<List<JsonElement>>() ?? new();
            }
            catch { return new(); }
        }

        public async Task<List<JsonElement>> GetByPelangganAsync(int pelangganId)
        {
            try
            {
                var res = await _http.GetAsync($"api/pesanan/pelanggan/{pelangganId}");
                if (!res.IsSuccessStatusCode) return new();
                return await res.Content.ReadFromJsonAsync<List<JsonElement>>() ?? new();
            }
            catch { return new(); }
        }

        public async Task<(bool sukses, string pesan)> BuatPesananAsync(
            int pelangganId, int obatId, int jumlah, string metode, int pembayaran)
        {
            try
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
                return (res.IsSuccessStatusCode, msg);
            }
            catch { return (false, "Tidak dapat terhubung ke server."); }
        }

        public async Task<(bool sukses, string pesan)> AktifkanTriggerAsync(int id, string trigger)
        {
            try
            {
                var res = await _http.PutAsJsonAsync($"api/pesanan/{id}/trigger",
                    new { trigger });
                string msg = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, msg);
            }
            catch { return (false, "Tidak dapat terhubung ke server."); }
        }

        public async Task<(bool sukses, string pesan)> HapusAsync(int id)
        {
            try
            {
                var res = await _http.DeleteAsync($"api/pesanan/{id}");
                string msg = await res.Content.ReadAsStringAsync();
                return (res.IsSuccessStatusCode, msg);
            }
            catch { return (false, "Tidak dapat terhubung ke server."); }
        }
    }
}