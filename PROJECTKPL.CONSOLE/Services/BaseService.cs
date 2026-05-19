using PROJECTKPL.API.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PROJECTKPL.CONSOLE.Services
{
    public abstract class BaseService<T>
    {
        protected readonly HttpClient _http;
        protected readonly JsonSerializerOptions _json;

        protected BaseService(HttpClient http)
        {
            _http = http;
            _json = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Generic GET list
        protected async Task<ApiResponse<List<T>>> GetListAsync(string url)
        {
            try
            {
                var res = await _http.GetAsync(url);
                if (!res.IsSuccessStatusCode)
                {
                    string err = await res.Content.ReadAsStringAsync();
                    return ApiResponse<List<T>>.Fail(err);
                }
                var list = await res.Content.ReadFromJsonAsync<List<T>>(_json);
                return ApiResponse<List<T>>.Ok(list ?? new());
            }
            catch (Exception ex)
            {
                return ApiResponse<List<T>>.Fail($"Koneksi gagal: {ex.Message}");
            }
        }

        // Generic GET single
        protected async Task<ApiResponse<T>> GetSingleAsync(string url)
        {
            try
            {
                var res = await _http.GetAsync(url);
                if (!res.IsSuccessStatusCode)
                {
                    string err = await res.Content.ReadAsStringAsync();
                    return ApiResponse<T>.Fail(err);
                }
                var item = await res.Content.ReadFromJsonAsync<T>(_json);
                return item != null
                    ? ApiResponse<T>.Ok(item)
                    : ApiResponse<T>.Fail("Data tidak ditemukan.");
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.Fail($"Koneksi gagal: {ex.Message}");
            }
        }

        // Generic POST
        protected async Task<ApiResponse<T>> PostAsync<TBody>(string url, TBody body)
        {
            try
            {
                var res = await _http.PostAsJsonAsync(url, body);
                string msg = await res.Content.ReadAsStringAsync();
                if (!res.IsSuccessStatusCode) return ApiResponse<T>.Fail(msg);

                var item = await res.Content.ReadFromJsonAsync<T>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return item != null ? ApiResponse<T>.Ok(item) : ApiResponse<T>.Fail(msg);
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.Fail($"Koneksi gagal: {ex.Message}");
            }
        }

        // Generic PUT
        protected async Task<ApiResponse<string>> PutAsync<TBody>(string url, TBody body)
        {
            try
            {
                var res = await _http.PutAsJsonAsync(url, body);
                string msg = await res.Content.ReadAsStringAsync();
                return res.IsSuccessStatusCode
                    ? ApiResponse<string>.Ok(msg)
                    : ApiResponse<string>.Fail(msg);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail($"Koneksi gagal: {ex.Message}");
            }
        }

        // Generic DELETE
        protected async Task<ApiResponse<string>> DeleteAsync(string url)
        {
            try
            {
                var res = await _http.DeleteAsync(url);
                string msg = await res.Content.ReadAsStringAsync();
                return res.IsSuccessStatusCode
                    ? ApiResponse<string>.Ok(msg)
                    : ApiResponse<string>.Fail(msg);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail($"Koneksi gagal: {ex.Message}");
            }
        }
    }
}
